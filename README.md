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

Enqueue: O(n), Calling thread must wait until after iterating the list for correct placement.<br />
EnqueueAsync: O(n), Calling thread immediately continues after offloading the work onto a worker thread.<br />
Dequeue: O(1), Calling thread must wait if the queue is empty but active enqueues are present.<br />
DequeueAsync: O(1), Calling thread immediately continues after offloading the work onto a worker thread.<br />
Peek: O(1), Calling thread must wait if the queue is empty but active enqueues are present.<br />

## Usage:

### Properties
<pre style="font-family:Cascadia Mono;font-size:13px;color:#dadada;background:#1e1e1e;"><span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">summary</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;The&nbsp;priority&nbsp;mode&nbsp;to&nbsp;set&nbsp;the&nbsp;priority&nbsp;queue.</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">summary</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#569cd6;">public</span>&nbsp;<span style="color:gainsboro;">PriorityType</span>&nbsp;<span style="color:gainsboro;">Mode</span></pre>

<pre style="font-family:Cascadia Mono;font-size:13px;color:#dadada;background:#1e1e1e;"><span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">summary</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;The&nbsp;number&nbsp;of&nbsp;values&nbsp;currently&nbsp;in&nbsp;the&nbsp;queue.</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">summary</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#569cd6;">public</span>&nbsp;<span style="color:#569cd6;">int</span>&nbsp;<span style="color:gainsboro;">Count</span></pre>

<pre style="font-family:Cascadia Mono;font-size:13px;color:#dadada;background:#1e1e1e;"><span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">summary</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;The&nbsp;number&nbsp;of&nbsp;asynchronous&nbsp;operations&nbsp;currently&nbsp;scheduled.</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">summary</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#569cd6;">public</span>&nbsp;<span style="color:#569cd6;">int</span>&nbsp;<span style="color:gainsboro;">AsyncEnqueueOperations</span></pre>

### Constructor
<pre style="font-family:Cascadia Mono;font-size:13px;color:#dadada;background:#1e1e1e;"><span style="color:#569cd6;">public</span>&nbsp;<span style="color:#4ec9b0;">ConcurrentPriorityQueue</span><span style="color:gainsboro;">(</span><span style="color:gainsboro;">PriorityType</span>&nbsp;<span style="color:#9cdcfe;">mode</span>&nbsp;<span style="color:#b4b4b4;">=</span>&nbsp;<span style="color:gainsboro;">PriorityType</span><span style="color:#b4b4b4;">.</span><span style="color:gainsboro;">Max</span><span style="color:gainsboro;">)</span>
</pre>

### Methods
<pre style="font-family:Cascadia Mono;font-size:13px;color:#dadada;background:#1e1e1e;"><span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">summary</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;Tries&nbsp;to&nbsp;add&nbsp;an&nbsp;entry&nbsp;to&nbsp;the&nbsp;priority&nbsp;queue.</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">summary</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">param</span>&nbsp;<span style="color:#c8c8c8;">name</span><span style="color:#608b4e;">=</span><span style="color:#c8c8c8;">&quot;</span><span style="color:gainsboro;">priority</span><span style="color:#c8c8c8;">&quot;</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">A&nbsp;comparable&nbsp;value&nbsp;to&nbsp;be&nbsp;used&nbsp;as&nbsp;the&nbsp;priority.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">param</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">param</span>&nbsp;<span style="color:#c8c8c8;">name</span><span style="color:#608b4e;">=</span><span style="color:#c8c8c8;">&quot;</span><span style="color:gainsboro;">value</span><span style="color:#c8c8c8;">&quot;</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">The&nbsp;value&nbsp;to&nbsp;be&nbsp;stored&nbsp;into&nbsp;the&nbsp;queue.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">param</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">param</span>&nbsp;<span style="color:#c8c8c8;">name</span><span style="color:#608b4e;">=</span><span style="color:#c8c8c8;">&quot;</span><span style="color:gainsboro;">allowDuplicates</span><span style="color:#c8c8c8;">&quot;</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">Whether&nbsp;or&nbsp;not&nbsp;to&nbsp;allow&nbsp;adding&nbsp;if&nbsp;an&nbsp;entry&nbsp;with&nbsp;the&nbsp;same&nbsp;priority&nbsp;already&nbsp;exists.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">param</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">returns</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">true&nbsp;if&nbsp;entry&nbsp;was&nbsp;successfully&nbsp;added&nbsp;to&nbsp;the&nbsp;queue,&nbsp;otherwise&nbsp;false.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">returns</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">remarks</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">This&nbsp;method&nbsp;blocks&nbsp;until&nbsp;the&nbsp;enqueue&nbsp;operation&nbsp;has&nbsp;completed.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">remarks</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#569cd6;">public</span>&nbsp;<span style="color:#569cd6;">bool</span>&nbsp;<span style="color:#dcdcaa;">TryEnqueue</span><span style="color:gainsboro;">(</span><span style="color:gainsboro;">P</span>&nbsp;<span style="color:#9cdcfe;">priority</span><span style="color:gainsboro;">,</span>&nbsp;<span style="color:gainsboro;">V</span>&nbsp;<span style="color:#9cdcfe;">value</span><span style="color:gainsboro;">,</span>&nbsp;<span style="color:#569cd6;">bool</span>&nbsp;<span style="color:#9cdcfe;">allowDuplicates</span>&nbsp;<span style="color:#b4b4b4;">=</span>&nbsp;<span style="color:#569cd6;">true</span><span style="color:gainsboro;">)</span></pre>

<pre style="font-family:Cascadia Mono;font-size:13px;color:#dadada;background:#1e1e1e;"><span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">summary</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;Asynchronously&nbsp;adds&nbsp;to&nbsp;the&nbsp;priority&nbsp;queue.</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">summary</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">param</span>&nbsp;<span style="color:#c8c8c8;">name</span><span style="color:#608b4e;">=</span><span style="color:#c8c8c8;">&quot;</span><span style="color:gainsboro;">priority</span><span style="color:#c8c8c8;">&quot;</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">A&nbsp;comparable&nbsp;value&nbsp;to&nbsp;be&nbsp;used&nbsp;as&nbsp;the&nbsp;priority.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">param</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">param</span>&nbsp;<span style="color:#c8c8c8;">name</span><span style="color:#608b4e;">=</span><span style="color:#c8c8c8;">&quot;</span><span style="color:gainsboro;">value</span><span style="color:#c8c8c8;">&quot;</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">The&nbsp;value&nbsp;to&nbsp;be&nbsp;stored&nbsp;into&nbsp;the&nbsp;queue.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">param</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">param</span>&nbsp;<span style="color:#c8c8c8;">name</span><span style="color:#608b4e;">=</span><span style="color:#c8c8c8;">&quot;</span><span style="color:gainsboro;">callback</span><span style="color:#c8c8c8;">&quot;</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">The&nbsp;callback&nbsp;to&nbsp;invoke&nbsp;once&nbsp;the&nbsp;enqueue&nbsp;operation&nbsp;has&nbsp;completed.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">param</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">remarks</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">This&nbsp;method&nbsp;does&nbsp;not&nbsp;block.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">remarks</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#569cd6;">public</span>&nbsp;<span style="color:#569cd6;">void</span>&nbsp;<span style="color:#dcdcaa;">TryEnqueueAsync</span><span style="color:gainsboro;">(</span><span style="color:gainsboro;">P</span>&nbsp;<span style="color:#9cdcfe;">priority</span><span style="color:gainsboro;">,</span>&nbsp;<span style="color:gainsboro;">V</span>&nbsp;<span style="color:#9cdcfe;">value</span><span style="color:gainsboro;">,</span>&nbsp;<span style="color:gainsboro;">Action</span><span style="color:gainsboro;">&lt;</span><span style="color:gainsboro;">AsyncPriorityOperationResult</span><span style="color:gainsboro;">&gt;</span><span style="color:#b4b4b4;">?</span>&nbsp;<span style="color:#9cdcfe;">callback</span>&nbsp;<span style="color:#b4b4b4;">=</span>&nbsp;<span style="color:#569cd6;">null</span><span style="color:gainsboro;">)</span></pre>

<pre style="font-family:Cascadia Mono;font-size:13px;color:#dadada;background:#1e1e1e;"><span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">summary</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;Asynchronously&nbsp;adds&nbsp;an&nbsp;entry&nbsp;to&nbsp;the&nbsp;priority&nbsp;queue.</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">summary</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">param</span>&nbsp;<span style="color:#c8c8c8;">name</span><span style="color:#608b4e;">=</span><span style="color:#c8c8c8;">&quot;</span><span style="color:gainsboro;">priority</span><span style="color:#c8c8c8;">&quot;</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">A&nbsp;comparable&nbsp;value&nbsp;to&nbsp;be&nbsp;used&nbsp;as&nbsp;the&nbsp;priority.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">param</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">param</span>&nbsp;<span style="color:#c8c8c8;">name</span><span style="color:#608b4e;">=</span><span style="color:#c8c8c8;">&quot;</span><span style="color:gainsboro;">value</span><span style="color:#c8c8c8;">&quot;</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">The&nbsp;value&nbsp;to&nbsp;be&nbsp;stored&nbsp;into&nbsp;the&nbsp;queue.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">param</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">param</span>&nbsp;<span style="color:#c8c8c8;">name</span><span style="color:#608b4e;">=</span><span style="color:#c8c8c8;">&quot;</span><span style="color:gainsboro;">allowDuplicates</span><span style="color:#c8c8c8;">&quot;</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">Whether&nbsp;or&nbsp;not&nbsp;to&nbsp;allow&nbsp;adding&nbsp;if&nbsp;an&nbsp;entry&nbsp;with&nbsp;the&nbsp;same&nbsp;priority&nbsp;already&nbsp;exists.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">param</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">param</span>&nbsp;<span style="color:#c8c8c8;">name</span><span style="color:#608b4e;">=</span><span style="color:#c8c8c8;">&quot;</span><span style="color:gainsboro;">callback</span><span style="color:#c8c8c8;">&quot;</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">The&nbsp;callback&nbsp;to&nbsp;invoke&nbsp;once&nbsp;the&nbsp;enqueue&nbsp;operation&nbsp;has&nbsp;completed.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">param</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">remarks</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">This&nbsp;method&nbsp;does&nbsp;not&nbsp;block.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">remarks</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#569cd6;">public</span>&nbsp;<span style="color:#569cd6;">void</span>&nbsp;<span style="color:#dcdcaa;">TryEnqueueAsync</span><span style="color:gainsboro;">(</span><span style="color:gainsboro;">P</span>&nbsp;<span style="color:#9cdcfe;">priority</span><span style="color:gainsboro;">,</span>&nbsp;<span style="color:gainsboro;">V</span>&nbsp;<span style="color:#9cdcfe;">value</span><span style="color:gainsboro;">,</span>&nbsp;<span style="color:#569cd6;">bool</span>&nbsp;<span style="color:#9cdcfe;">allowDuplicates</span><span style="color:gainsboro;">,</span>&nbsp;<span style="color:gainsboro;">Action</span><span style="color:gainsboro;">&lt;</span><span style="color:gainsboro;">AsyncPriorityOperationResult</span><span style="color:gainsboro;">&gt;</span><span style="color:#b4b4b4;">?</span>&nbsp;<span style="color:#9cdcfe;">callback</span>&nbsp;<span style="color:#b4b4b4;">=</span>&nbsp;<span style="color:#569cd6;">null</span><span style="color:gainsboro;">)</span></pre>

<pre style="font-family:Cascadia Mono;font-size:13px;color:#dadada;background:#1e1e1e;"><span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">summary</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;Tries&nbsp;to&nbsp;retrieve&nbsp;and&nbsp;remove&nbsp;from&nbsp;the&nbsp;priority&nbsp;queue.</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">summary</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">param</span>&nbsp;<span style="color:#c8c8c8;">name</span><span style="color:#608b4e;">=</span><span style="color:#c8c8c8;">&quot;</span><span style="color:gainsboro;">value</span><span style="color:#c8c8c8;">&quot;</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">The&nbsp;value&nbsp;which&nbsp;has&nbsp;been&nbsp;dequeued.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">param</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">returns</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">true&nbsp;if&nbsp;an&nbsp;entry&nbsp;has&nbsp;been&nbsp;successfully&nbsp;dequeued,&nbsp;otherwise&nbsp;false.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">returns</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">remarks</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">This&nbsp;method&nbsp;blocks&nbsp;until&nbsp;the&nbsp;dequeue&nbsp;operation&nbsp;has&nbsp;completed.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">remarks</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#569cd6;">public</span>&nbsp;<span style="color:#569cd6;">bool</span>&nbsp;<span style="color:#dcdcaa;">TryDequeue</span><span style="color:gainsboro;">(</span><span style="color:#569cd6;">out</span>&nbsp;<span style="color:gainsboro;">V</span>&nbsp;<span style="color:#9cdcfe;">value</span><span style="color:gainsboro;">)</span></pre>

<pre style="font-family:Cascadia Mono;font-size:13px;color:#dadada;background:#1e1e1e;"><span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">summary</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;Asyncronously&nbsp;dequeues&nbsp;an&nbsp;entry&nbsp;from&nbsp;the&nbsp;priority&nbsp;queue.</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">summary</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">param</span>&nbsp;<span style="color:#c8c8c8;">name</span><span style="color:#608b4e;">=</span><span style="color:#c8c8c8;">&quot;</span><span style="color:gainsboro;">callback</span><span style="color:#c8c8c8;">&quot;</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">The&nbsp;callback&nbsp;delegate&nbsp;to&nbsp;invoke&nbsp;after&nbsp;dequeuing&nbsp;has&nbsp;completed.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">param</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">remarks</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">This&nbsp;method&nbsp;does&nbsp;not&nbsp;block.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">remarks</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#569cd6;">public</span>&nbsp;<span style="color:#569cd6;">void</span>&nbsp;<span style="color:#dcdcaa;">TryDequeueAsync</span><span style="color:gainsboro;">(</span><span style="color:gainsboro;">Action</span><span style="color:gainsboro;">&lt;</span><span style="color:gainsboro;">AsyncPriorityOperationResult</span><span style="color:gainsboro;">&gt;</span><span style="color:#b4b4b4;">?</span>&nbsp;<span style="color:#9cdcfe;">callback</span>&nbsp;<span style="color:#b4b4b4;">=</span>&nbsp;<span style="color:#569cd6;">null</span><span style="color:gainsboro;">)</span></pre>

<pre style="font-family:Cascadia Mono;font-size:13px;color:#dadada;background:#1e1e1e;"><span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">summary</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;Tries&nbsp;to&nbsp;retrieve&nbsp;without&nbsp;removing&nbsp;from&nbsp;the&nbsp;priority&nbsp;queue.</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">summary</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">param</span>&nbsp;<span style="color:#c8c8c8;">name</span><span style="color:#608b4e;">=</span><span style="color:#c8c8c8;">&quot;</span><span style="color:gainsboro;">value</span><span style="color:#c8c8c8;">&quot;</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">The&nbsp;value&nbsp;at&nbsp;the&nbsp;beginning&nbsp;of&nbsp;the&nbsp;priority&nbsp;queue.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">param</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">returns</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">true&nbsp;if&nbsp;an&nbsp;entry&nbsp;has&nbsp;been&nbsp;successfully&nbsp;retrieved,&nbsp;otherwise&nbsp;false.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">returns</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">remarks</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">This&nbsp;method&nbsp;blocks&nbsp;until&nbsp;the&nbsp;peek&nbsp;operation&nbsp;has&nbsp;completed.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">remarks</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#569cd6;">public</span>&nbsp;<span style="color:#569cd6;">bool</span>&nbsp;<span style="color:#dcdcaa;">TryPeek</span><span style="color:gainsboro;">(</span><span style="color:#569cd6;">out</span>&nbsp;<span style="color:gainsboro;">V</span><span style="color:#b4b4b4;">?</span>&nbsp;<span style="color:#9cdcfe;">value</span><span style="color:gainsboro;">)</span></pre>

<pre style="font-family:Cascadia Mono;font-size:13px;color:#dadada;background:#1e1e1e;"><span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">summary</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;Blocks&nbsp;until&nbsp;all&nbsp;asynchronous&nbsp;enqueue&nbsp;opertions&nbsp;are&nbsp;completed.</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">summary</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">remarks</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">This&nbsp;method&nbsp;blocks&nbsp;until&nbsp;there&nbsp;are&nbsp;no&nbsp;more&nbsp;active&nbsp;asynchronous&nbsp;enqueues</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">remarks</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#569cd6;">public</span>&nbsp;<span style="color:#569cd6;">void</span>&nbsp;<span style="color:#dcdcaa;">WaitForAsyncEnqueues</span><span style="color:gainsboro;">(</span><span style="color:#569cd6;">int</span>&nbsp;<span style="color:#9cdcfe;">millisecondsTimeout</span>&nbsp;<span style="color:#b4b4b4;">=</span>&nbsp;<span style="color:#b5cea8;">0</span><span style="color:gainsboro;">)</span></pre>

<pre style="font-family:Cascadia Mono;font-size:13px;color:#dadada;background:#1e1e1e;"><span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">summary</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;Clears&nbsp;the&nbsp;entries&nbsp;currently&nbsp;in&nbsp;the&nbsp;queue.</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">summary</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#608b4e;">///</span><span style="color:#608b4e;">&nbsp;</span><span style="color:#608b4e;">&lt;</span><span style="color:#608b4e;">remarks</span><span style="color:#608b4e;">&gt;</span><span style="color:#608b4e;">Asynchronous&nbsp;enqueues&nbsp;after&nbsp;this&nbsp;method&nbsp;will&nbsp;still&nbsp;be&nbsp;added.</span><span style="color:#608b4e;">&lt;/</span><span style="color:#608b4e;">remarks</span><span style="color:#608b4e;">&gt;</span>
<span style="color:#569cd6;">public</span>&nbsp;<span style="color:#569cd6;">void</span>&nbsp;<span style="color:#dcdcaa;">Clear</span><span style="color:gainsboro;">()</span></pre>

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

Asynchronous operations can be made with the TyEnqueueAsync() and TryDequeueAsync() methods.

```csharp
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
```

Enqueues can run in parallel.

```csharp
// Parallel
Random rnd = new();
Parallel.For(0, 10000, i =>
{
    int n = rnd.Next(0, 100000);
    priorityQ.TryEnqueueAsync(n, n.ToString());
});

// Waits for all asynchronous enqueues to complete with a 2 second timeout
// Initially checks and if async enqueues are complete, wait 2 seconds and checks again
priorityQ.WaitForAsyncEnqueues(2000);

foreach (string str in priorityQ)
    Console.WriteLine(str);

Console.WriteLine($"Entries: {priorityQ.Count}");

await Task.Delay(-1);
```