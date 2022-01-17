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

Console.WriteLine($"Async Enqueues left: {priorityQ.AsyncEnqueueOperations}");

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