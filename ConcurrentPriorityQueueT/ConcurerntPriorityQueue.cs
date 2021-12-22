using System.Collections.Generic;
using System.Threading;

namespace System.Collections.Concurrent;

/// <summary>
/// Types of priority modes.
/// </summary>
public enum PriorityMode : int
{
    /// <summary>
    /// Max priorities come first.
    /// </summary>
    Max = -1,
    /// <summary>
    /// Min priorities come first.
    /// </summary>
    Min = 1,
}

/// <summary>
/// A concurrent priority queue with parallelism.
/// </summary>
/// <typeparam name="P">The type which implement IComparable to use as the priority.</typeparam>
/// <typeparam name="V">The type which this colllection will store.</typeparam>
public class ConcurrentPriorityQueue<P, V> : IEnumerable<V> where P : IComparable<P>
{
    /// <summary>
    /// The result of an asynchronouse operation once it has completed.
    /// </summary>
    public class AsyncPriorityOperationResult
    {
        /// <summary>
        /// The success status of an asynchronous operation.
        /// </summary>
        public readonly bool Success;
        /// <summary>
        /// The resulting priority of an asynchronous operation.
        /// </summary>
        public readonly P Priority;
        /// <summary>
        /// The resulting value of an asynchronous operation.
        /// </summary>
        public readonly V Value;
        internal AsyncPriorityOperationResult(bool success, P priority, V value)
        {
            Success = success;
            Priority = priority;
            Value = value;
        }
        /// <summary>
        /// AsyncPriorityOperationResult can be implicitly used where boolean values are expected.
        /// </summary>
        /// <param name="r">The AsyncPriorityOperationResult object.</param>
        public static implicit operator bool(AsyncPriorityOperationResult r) => r.Success;
    }

    internal class PriorityNode
    {
        internal readonly P _priority;
        internal readonly V _value;
        internal PriorityNode? _next;
        internal int _lock;
        public PriorityNode(P priority, V value, PriorityNode? next = null)
        {
            _priority = priority;
            _value = value;
            _next = next;
        }
    }

    private readonly PriorityNode _head;
    private int _count = 0;
    private int _asyncEnqueueOperations = 0;

    /// <summary>
    /// The priority mode to set the priority queue.
    /// </summary>
    public PriorityMode Mode { get; private set; }
    /// <summary>
    /// The number of values currently in the queue.
    /// </summary>
    public int Count => _count;
    /// <summary>
    /// The number of asynchronous operations currently scheduled.
    /// </summary>
    public int AsyncEnqueueOperations => _asyncEnqueueOperations;

    /// <summary>
    /// Creates a new concurrent priority queue.
    /// </summary>
    /// <param name="mode">The type of priority to use for the queue.</param>
    public ConcurrentPriorityQueue(PriorityMode mode = PriorityMode.Max)
    {
        Mode = mode;
#pragma warning disable CS8604 // Possible null reference argument.
        _head = new(default, default);
#pragma warning restore CS8604 // Possible null reference argument.
    }

    /// <summary>
    /// Clears the entries currently in the queue.
    /// </summary>
    /// <remarks>Asynchronously enqueues after this method being called will still be added.</remarks>
    public void Clear()
    {
        while (Interlocked.Exchange(ref _head._lock, 1) == 1) ;
        _head._next = null;
        Interlocked.Exchange(ref _count, 0);
        Interlocked.Exchange(ref _head._lock, 0);
    }

    /// <summary>
    /// Tries to add an entry to the priority queue.
    /// </summary>
    /// <param name="priority">A comparable value to be used as the priority.</param>
    /// <param name="value">The value to be stored into the queue.</param>
    /// <param name="allowDuplicates">Whether or not to allow adding if an entry with the same priority already exists.</param>
    /// <returns>true if entry was successfully added to the queue, otherwise false.</returns>
    /// <remarks>This method blocks until the enqueue operation has completed.</remarks>
    public bool TryEnqueue(P priority, V value, bool allowDuplicates = true)
    {
        while (Interlocked.Exchange(ref _head._lock, 1) == 1) ;
        PriorityNode crntNode = _head;
        while (crntNode._next != null)
        {
            int comparison = crntNode._next._priority.CompareTo(priority);
            if (comparison == (int)Mode)
            {
                break;
            }
            else if (comparison == 0 && !allowDuplicates)
            {
                Interlocked.Exchange(ref crntNode._lock, 0);
                return false;
            }
            else
            {
                PriorityNode prevNode = crntNode;
                while (Interlocked.Exchange(ref crntNode._next._lock, 1) == 1) ;
                crntNode = crntNode._next;
                Interlocked.Exchange(ref prevNode._lock, 0);
            }
        }
        crntNode._next = new PriorityNode(priority, value, crntNode._next);
        Interlocked.Increment(ref _count);
        Interlocked.Exchange(ref crntNode._lock, 0);
        return true;
    }

    /// <summary>
    /// Asynchronously adds to the priority queue.
    /// </summary>
    /// <param name="priority">A comparable value to be used as the priority.</param>
    /// <param name="value">The value to be stored into the queue.</param>
    /// <param name="callback">The callback to invoke once the enqueue operation has completed.</param>
    /// <remarks>This method does not block.</remarks>
    public void TryEnqueueAsync(P priority, V value, Action<AsyncPriorityOperationResult>? callback = null) =>
        TryEnqueueAsync(priority, value, true, callback);
    /// <summary>
    /// Asynchronously adds an entry to the priority queue.
    /// </summary>
    /// <param name="priority">A comparable value to be used as the priority.</param>
    /// <param name="value">The value to be stored into the queue.</param>
    /// <param name="allowDuplicates">Whether or not to allow adding if an entry with the same priority already exists.</param>
    /// <param name="callback">The callback to invoke once the enqueue operation has completed.</param>
    /// <remarks>This method does not block.</remarks>
    public void TryEnqueueAsync(P priority, V value, bool allowDuplicates, Action<AsyncPriorityOperationResult>? callback = null)
    {
        Interlocked.Increment(ref _asyncEnqueueOperations);
        ThreadPool.QueueUserWorkItem(_ =>
        {
            //Interlocked.Increment(ref _activeAsyncOperations);
            while (Interlocked.Exchange(ref _head._lock, 1) == 1) ;
            PriorityNode crntNode = _head;
            while (crntNode._next != null)
            {
                int comparison = crntNode._next._priority.CompareTo(priority);
                if (comparison == (int)Mode)
                {
                    break;
                }
                else if (comparison == 0 && !allowDuplicates)
                {
                    Interlocked.Decrement(ref _asyncEnqueueOperations);
                    Interlocked.Exchange(ref crntNode._lock, 0);
                    callback?.Invoke(new(false, priority, value));
                    return;
                }
                else
                {
                    PriorityNode prevNode = crntNode;
                    while (Interlocked.Exchange(ref crntNode._next._lock, 1) == 1) ;
                    crntNode = crntNode._next;
                    Interlocked.Exchange(ref prevNode._lock, 0);
                }
            }
            crntNode._next = new PriorityNode(priority, value, crntNode._next);
            Interlocked.Increment(ref _count);
            Interlocked.Decrement(ref _asyncEnqueueOperations);
            Interlocked.Exchange(ref crntNode._lock, 0);
            callback?.Invoke(new(true, priority, value));
        });
    }

    /// <summary>
    /// Tries to retrieve and remove from the priority queue.
    /// </summary>
    /// <param name="value">The value which has been dequeued.</param>
    /// <returns>true if an entry has been successfully dequeued, otherwise false.</returns>
    /// <remarks>This method blocks until the dequeue operation has completed.</remarks>
    public bool TryDequeue(out V? value)
    {
        while (true)
        {
            while (Interlocked.Exchange(ref _head._lock, 1) == 1) ;
            if (_head._next == null)
            {
                Interlocked.Exchange(ref _head._lock, 0);
                if (_asyncEnqueueOperations > 0) continue;
                value = default;
                return false;
            }
            else
            {
                while (Interlocked.Exchange(ref _head._next._lock, 1) == 1) ;
                value = _head._next._value;
                _head._next = _head._next._next;
                Interlocked.Decrement(ref _count);
                Interlocked.Exchange(ref _head._lock, 0);
                return true;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="callback"></param>
    public void TryDequeueAsync(Action<AsyncPriorityOperationResult>? callback = null)
    {
        _ = ThreadPool.QueueUserWorkItem(_ =>
          {
              while (true)
              {
                  while (Interlocked.Exchange(ref _head._lock, 1) == 1) ;
                  if (_head._next == null)
                  {
                      Interlocked.Exchange(ref _head._lock, 0);
                      if (_asyncEnqueueOperations > 0) continue;
#pragma warning disable CS8604 // Possible null reference argument.
                      callback?.Invoke(new(false, default, default));
#pragma warning restore CS8604 // Possible null reference argument.
                      return;
                  }
                  else
                  {
                      while (Interlocked.Exchange(ref _head._next._lock, 1) == 1) ;
                      AsyncPriorityOperationResult result = new(true, _head._next._priority, _head._next._value);
                      _head._next = _head._next._next;
                      Interlocked.Decrement(ref _count);
                      Interlocked.Exchange(ref _head._lock, 0);
                      callback?.Invoke(result);
                      return;
                  }
              }
          });
    }

    /// <summary>
    /// Tries to retrieve without removing from the priority queue.
    /// </summary>
    /// <param name="value">The value at the beginning of the priority queue.</param>
    /// <returns>true if an entry has been successfully retrieved, otherwise false.</returns>
    /// <remarks>This method blocks until the peek operation has completed.</remarks>
    public bool TryPeek(out V? value)
    {
        while (true)
        {
            while (Interlocked.Exchange(ref _head._lock, 1) == 1) ;
            if (_head._next == null)
            {
                Interlocked.Exchange(ref _head._lock, 0);
                if (_asyncEnqueueOperations > 0) continue;
                value = default;
                return false;
            }
            else
            {
                while (Interlocked.Exchange(ref _head._next._lock, 1) == 1) ;
                value = _head._next._value;
                Interlocked.Decrement(ref _count);
                Interlocked.Exchange(ref _head._lock, 0);
                return true;
            }
        }
    }

    /// <summary>
    /// Blocks until all asynchronous enqueue opertions are completed.
    /// </summary>
    public void WaitForAsyncEnqueues(int millisecondsTimeout = 0)
    {
        bool wait = true;
        while (wait)
        {
            while (Interlocked.Exchange(ref _head._lock, 1) == 1) ;
            if (_asyncEnqueueOperations == 0)
            {
                Thread.Sleep(millisecondsTimeout);
                if (_asyncEnqueueOperations == 0)
                    wait = false;
            }
            Interlocked.Exchange(ref _head._lock, 0);
        }
    }

    /// <summary>
    /// Iterate through entries currently in the priority queue.
    /// </summary>
    /// <returns>An enumerator through the priority queue.</returns>
    /// <remarks>This does not include entries yet to be enqueued asynchronously.</remarks>
    public IEnumerator<V> GetEnumerator()
    {
        while (Interlocked.Exchange(ref _head._lock, 1) == 1) ;
        PriorityNode crntNode = _head;
        while (crntNode._next != null)
        {
            while (Interlocked.Exchange(ref crntNode._next._lock, 1) == 1) ;
            PriorityNode prevNode = crntNode;
            crntNode = crntNode._next;
            Interlocked.Exchange(ref prevNode._lock, 0);
            yield return crntNode._value;
        }
        Interlocked.Exchange(ref crntNode._lock, 0);
    }
    /// <summary>
    /// Iterate through entries currently in the priority queue.
    /// </summary>
    /// <returns>An enumerator through the priority queue.</returns>
    /// <remarks>This does not include entries yet to be enqueued asynchronously.</remarks>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}