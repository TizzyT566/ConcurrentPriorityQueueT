# ConcurrentPriorityQueueT
A concurrent priority queue with parallelism.

Nuget Package: [https://www.nuget.org/packages/ConcurrentPriorityQueueT/](https://www.nuget.org/packages/ConcurrentPriorityQueueT/)

## Features

1. Fully concurrent, All operations are thread-safe.
2. Parallel enqueuing (threads do not lock the entire collection when enqueuing).
3. Specifying the type of priority queue (Min or Max).
4. Specifying if duplicates are allowed on enqueuing.
5. Utilizes the asynchronous programming model (APM) to invoke callbacks for asynchronous methods.

## Complexity

- Enqueue: O(n) - Calling thread must wait until after iterating the list for correct placement.
- EnqueueAsync: O(n) - Calling thread immediately continues after offloading the work onto a worker thread.
- Dequeue: O(1) - Calling thread must wait if the queue is empty but active enqueues are present.
- DequeueAsync: O(1) - Calling thread immediately continues after offloading the work onto a worker thread.
- Peek: O(1) - Calling thread must wait if the queue is empty but active enqueues are present.
- Clear: O(1)

## Usage:

### Properties
```csharp
/// <summary>
/// The priority mode to set the priority queue.
/// </summary>
public PriorityType Mode
```
```csharp
/// <summary>
/// The number of values currently in the queue.
/// </summary>
public int Count
```
```csharp
/// <summary>
/// The number of asynchronous operations currently scheduled.
/// </summary>
public int AsyncEnqueueOperations
```

### Constructor
```csharp
/// <summary>
/// Creates a new concurrent priority queue.
/// </summary>
/// <param name="mode">The type of priority to use for the queue.</param>
public ConcurrentPriorityQueue(PriorityType mode = PriorityType.Max)
```

### Methods
```csharp
/// <summary>
/// Tries to add an entry to the priority queue.
/// </summary>
/// <param name="priority">A comparable value to be used as the priority.</param>
/// <param name="value">The value to be stored into the queue.</param>
/// <param name="allowDuplicates">Whether or not to allow adding duplicate entry.</param>
/// <returns>true if entry was successfully added to the queue, otherwise false.</returns>
/// <remarks>This method blocks until the enqueue operation has completed.</remarks>
public bool TryEnqueue(P priority, V value, bool allowDuplicates = true)
```
```csharp
/// <summary>
/// Asynchronously adds to the priority queue.
/// </summary>
/// <param name="priority">A comparable value to be used as the priority.</param>
/// <param name="value">The value to be stored into the queue.</param>
/// <param name="callback">The callback to invoke once the enqueue operation has completed.</param>
/// <remarks>This method does not block.</remarks>
public void TryEnqueueAsync(P priority, V value, Action<AsyncPriorityOperationResult>? callback = null)
```
```csharp
/// <summary>
/// Asynchronously adds an entry to the priority queue.
/// </summary>
/// <param name="priority">A comparable value to be used as the priority.</param>
/// <param name="value">The value to be stored into the queue.</param>
/// <param name="allowDuplicates">Whether or not to allow adding duplicate entry.</param>
/// <param name="callback">The callback to invoke once the enqueue operation has completed.</param>
/// <remarks>This method does not block.</remarks>
public void TryEnqueueAsync(P priority, V value, bool allowDuplicates, Action<AsyncPriorityOperationResult>? callback = null)
```
```csharp
/// <summary>
/// Tries to retrieve and remove from the priority queue.
/// </summary>
/// <param name="value">The value which has been dequeued.</param>
/// <returns>true if an entry has been successfully dequeued, otherwise false.</returns>
/// <remarks>This method blocks until the dequeue operation has completed.</remarks>
public bool TryDequeue(out V value)
```
```csharp
/// <summary>
/// Asyncronously dequeues an entry from the priority queue.
/// </summary>
/// <param name="callback">The callback delegate to invoke after dequeuing has completed.</param>
/// <remarks>This method does not block.</remarks>
public void TryDequeueAsync(Action<AsyncPriorityOperationResult>? callback = null)
```
```csharp
/// <summary>
/// Tries to retrieve without removing from the priority queue.
/// </summary>
/// <param name="value">The value at the beginning of the priority queue.</param>
/// <returns>true if an entry has been successfully retrieved, otherwise false.</returns>
/// <remarks>This method blocks until the peek operation has completed.</remarks>
public bool TryPeek(out V? value)
```
```csharp
/// <summary>
/// Blocks until all asynchronous enqueue opertions are completed.
/// </summary>
/// <param name="millisecondsGraceTime">Time after initial check to double check.</param>
/// <param name="millisecondTimeout">Total time to wait for async enqueues.</param>
/// <returns>true if enqueues have been completed for a specified grace time, false if timed out before enqueues finished.</returns>
/// <remarks>This method blocks until there are no more active asynchronous enqueues, or times out.</remarks>
public bool WaitForAsyncEnqueues(double millisecondsGraceTime = 0, int millisecondTimeout = -1)
```
```csharp
/// <summary>
/// Clears the entries currently in the queue.
/// </summary>
/// <remarks>Asynchronous enqueues after this method will still be added.</remarks>
public void Clear()
```

### Examples:

The data type of P in ConcurrentPriorityQueue<P, V> must implement IComparable<P>
```csharp
using System.Collections.Concurrent;

// Create a min type priority queue where 'int' is used for the priority and 'string' is used as the value type
ConcurrentPriorityQueue<int, string> priorityQ = new(PriorityType.Min);
```

Synchronous operations can be made with the TryEnqueue() and TryDequeue() methods.

```csharp
// Enqueue synchronously disallowing duplicates
if (priorityQ.TryEnqueue(1, "World", false)) Console.WriteLine("Enqueue success"); else Console.WriteLine("Duplicate entry");
if (priorityQ.TryEnqueue(1, "World", false)) Console.WriteLine("Enqueue success"); else Console.WriteLine("Duplicate entry");
if (priorityQ.TryEnqueue(0, "Hello", false)) Console.WriteLine("Enqueue success"); else Console.WriteLine("Duplicate entry");

Console.WriteLine();

// Dequeue synchronously
if (priorityQ.TryDequeue(out string value1)) Console.WriteLine(value1); else Console.WriteLine("Queue is empty");
if (priorityQ.TryDequeue(out string value2)) Console.WriteLine(value2); else Console.WriteLine("Queue is empty");
if (priorityQ.TryDequeue(out string value3)) Console.WriteLine(value3); else Console.WriteLine("Queue is empty");
```

Asynchronous operations can be made with the TryEnqueueAsync() and TryDequeueAsync() methods.

```csharp
using System.Collections.Concurrent;
using System.Text;

// Create a min type priority queue
ConcurrentPriorityQueue<int, string> priorityQ = new(PriorityType.Min);

// Enqueue synchronously disallowing duplicates
if (priorityQ.TryEnqueue(1, "World", false)) Console.WriteLine("Enqueue success"); else Console.WriteLine("Duplicate entry");
if (priorityQ.TryEnqueue(1, "World", false)) Console.WriteLine("Enqueue success"); else Console.WriteLine("Duplicate entry");
if (priorityQ.TryEnqueue(0, "Hello", false)) Console.WriteLine("Enqueue success"); else Console.WriteLine("Duplicate entry");

Console.WriteLine();

// Dequeue synchronously
if (priorityQ.TryDequeue(out string value1)) Console.WriteLine(value1); else Console.WriteLine("Queue is empty");
if (priorityQ.TryDequeue(out string value2)) Console.WriteLine(value2); else Console.WriteLine("Queue is empty");
if (priorityQ.TryDequeue(out string value3)) Console.WriteLine(value3); else Console.WriteLine("Queue is empty");


// Enqueue asynchronously
priorityQ.TryEnqueueAsync(0, "Hello World", false, r =>
{
    if (r)
        Console.WriteLine($"Enqueued: {r.Value}");
    else
        Console.WriteLine("Enqueue dup");
});
priorityQ.TryEnqueueAsync(0, "Hello World", false, r =>
{
    if (r)
        Console.WriteLine($"Enqueued: {r.Value}");
    else
        Console.WriteLine("Enqueue dup");
});

Console.WriteLine();

// Dequeue asynchronously
priorityQ.TryDequeueAsync(r =>
{
    if (r)
        Console.WriteLine($"Dequeued: {r.Value}");
    else
        Console.WriteLine("Queue is empty");
});
priorityQ.TryDequeueAsync(r =>
{
    if (r)
        Console.WriteLine($"Dequeued: {r.Value}");
    else
        Console.WriteLine("Queue is empty");
});


// Multiple TryEnqueueAsync calls run in parallel
Random rnd = new();
int count = 100;
for (int i = 0; i < count; i++)
{
    priorityQ.TryEnqueueAsync(i, i.ToString());
}

Console.WriteLine(priorityQ.Count);

// Waits for all asynchronous enqueues to complete and then ensure
// its empty for at least 2 seconds else times out in 5 seconds.
while (!priorityQ.WaitForAsyncEnqueues(2000, 5000))
    Console.WriteLine("\nTimed out before enqueues finished.\n");

Console.WriteLine("\nAll Async Enqueues compeleted.\n");

StringBuilder sb = new();
foreach (string str in priorityQ)
    sb.AppendLine(str);

Console.WriteLine(sb.ToString());

Console.WriteLine($"Entries: {priorityQ.Count}");

await Task.Delay(-1);
```

### Note

TryEnqueueAsync already creates it's own thread,

Do not place TryEnqueueAsync calls inside of user created threads or through apis for System.Threading.Tasks.Parallel.

Doing so may lead poor performance due to thread exhuastion.