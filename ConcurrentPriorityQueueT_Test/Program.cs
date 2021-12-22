using System.Collections.Concurrent;

// Create a min type priority queue
ConcurrentPriorityQueue<int, string> priorityQ = new(PriorityMode.Min);

// Enqueue synchronously disallowing duplicates
if (priorityQ.TryEnqueue(1, "World", false)) Console.WriteLine("Enqueue success"); else Console.WriteLine("Duplicate entry");
if (priorityQ.TryEnqueue(1, "World", false)) Console.WriteLine("Enqueue success"); else Console.WriteLine("Duplicate entry");
if (priorityQ.TryEnqueue(0, "Hello", false)) Console.WriteLine("Enqueue success"); else Console.WriteLine("Duplicate entry");

Console.WriteLine();

// Dequeue synchronously
if (priorityQ.TryDequeue(out string? value1)) Console.WriteLine(value1); else Console.WriteLine("Queue is empty");
if (priorityQ.TryDequeue(out string? value2)) Console.WriteLine(value2); else Console.WriteLine("Queue is empty");
if (priorityQ.TryDequeue(out string? value3)) Console.WriteLine(value3); else Console.WriteLine("Queue is empty");


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

// Parallel
Random rnd = new();
Parallel.For(0, 10000, i =>
{
    int n = rnd.Next(0, 100000);
    priorityQ.TryEnqueueAsync(n, n.ToString());
});

// Waits for all asynchronous enqueues to complete
Console.WriteLine($"Async Enqueues left: {priorityQ.AsyncEnqueueOperations}");
priorityQ.WaitForAsyncEnqueues(2000);
Console.WriteLine($"Async Enqueues left: {priorityQ.AsyncEnqueueOperations}");

Console.WriteLine();

foreach (string str in priorityQ)
    Console.WriteLine(str);

Console.WriteLine();

Console.WriteLine($"Entries: {priorityQ.Count}");

await Task.Delay(-1);