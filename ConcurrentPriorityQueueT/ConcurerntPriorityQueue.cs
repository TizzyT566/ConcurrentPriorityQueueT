using System.Collections.Generic;
using System.Threading;

namespace System.Collections.Concurrent;

/// <summary>
/// Types of priority types.
/// </summary>
public enum PriorityType : int
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

    private class PriorityNode
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
    /// The priority type to set the priority queue.
    /// </summary>
    public PriorityType Type { get; private set; }
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
    /// <param name="type">The type of priority to use for the queue.</param>
    public ConcurrentPriorityQueue(PriorityType type = PriorityType.Max)
    {
        Type = type;
#pragma warning disable CS8604 // Possible null reference argument.
        _head = new(default, default);
#pragma warning restore CS8604 // Possible null reference argument.
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
            if (comparison == (int)Type)
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
            bool result = TryEnqueue(priority, value, allowDuplicates);
            callback?.Invoke(new(result, priority, value));
            Interlocked.Decrement(ref _asyncEnqueueOperations);
        });
    }

    /// <summary>
    /// Tries to retrieve and remove from the priority queue.
    /// </summary>
    /// <param name="value">The value which has been dequeued.</param>
    /// <returns>true if an entry has been successfully dequeued, otherwise false.</returns>
    /// <remarks>This method blocks until the dequeue operation has completed.</remarks>
    public bool TryDequeue(out V value)
    {
        while (true)
        {
            while (Interlocked.Exchange(ref _head._lock, 1) == 1) ;
            if (_head._next == null)
            {
                Interlocked.Exchange(ref _head._lock, 0);
                if (_asyncEnqueueOperations > 0) continue;
#pragma warning disable CS8601 // Possible null reference assignment.
                value = default;
#pragma warning restore CS8601 // Possible null reference assignment.
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
    /// Asyncronously dequeues an entry from the priority queue.
    /// </summary>
    /// <param name="callback">The callback delegate to invoke after dequeuing has completed.</param>
    /// <remarks>This method does not block.</remarks>
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
    /// <param name="millisecondsGraceTime">Time after initial check to double check.</param>
    /// <param name="millisecondTimeout">Total time to wait for async enqueues.</param>
    /// <returns>true if enqueues have been completed for a specified grace time, false if timed out before enqueues finished.</returns>
    /// <remarks>This method blocks until there are no more active asynchronous enqueues, or times out.</remarks>
    public bool WaitForAsyncEnqueues(double millisecondsGraceTime = 0, int millisecondTimeout = -1)
    {
        bool wait = false;
        long timeStart = 0;
        long graceTicks = TimeSpan.FromMilliseconds(millisecondsGraceTime).Ticks;
        return SpinWait.SpinUntil(() =>
        {
            if (Volatile.Read(ref _asyncEnqueueOperations) == 0)
            {
                long timeStamp = Diagnostics.Stopwatch.GetTimestamp();
                if (!wait)
                {
                    wait = true;
                    timeStart = timeStamp;
                }
                if ((timeStamp - timeStart) >= graceTicks)
                    return true;
            }
            else
                wait = false;
            return false;
        }, millisecondTimeout);
    }

    /// <summary>
    /// Clears the entries currently in the queue.
    /// </summary>
    /// <remarks>Asynchronous enqueues after this method will still be added.</remarks>
    public void Clear()
    {
        while (Interlocked.Exchange(ref _head._lock, 1) == 1) ;
        _head._next = null;
        Interlocked.Exchange(ref _count, 0);
        Interlocked.Exchange(ref _head._lock, 0);
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