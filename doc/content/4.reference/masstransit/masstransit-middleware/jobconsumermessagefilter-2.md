---

title: JobConsumerMessageFilter<TConsumer, TJob>

---

# JobConsumerMessageFilter\<TConsumer, TJob\>

Namespace: MassTransit.Middleware

Converts the ConsumeContext to a JobContext, and executes the job

```csharp
public class JobConsumerMessageFilter<TConsumer, TJob> : IConsumerMessageFilter<TConsumer, TJob>, IFilter<ConsumerConsumeContext<TConsumer, TJob>>, IProbeSite
```

#### Type Parameters

`TConsumer`<br/>
The consumer type

`TJob`<br/>
The message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobConsumerMessageFilter\<TConsumer, TJob\>](../masstransit-middleware/jobconsumermessagefilter-2)<br/>
Implements [IConsumerMessageFilter\<TConsumer, TJob\>](../masstransit-middleware/iconsumermessagefilter-2), [IFilter\<ConsumerConsumeContext\<TConsumer, TJob\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **JobConsumerMessageFilter(IRetryPolicy)**

```csharp
public JobConsumerMessageFilter(IRetryPolicy retryPolicy)
```

#### Parameters

`retryPolicy` [IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Send(ConsumerConsumeContext\<TConsumer, TJob\>, IPipe\<ConsumerConsumeContext\<TConsumer, TJob\>\>)**

```csharp
public Task Send(ConsumerConsumeContext<TConsumer, TJob> context, IPipe<ConsumerConsumeContext<TConsumer, TJob>> next)
```

#### Parameters

`context` [ConsumerConsumeContext\<TConsumer, TJob\>](../../masstransit-abstractions/masstransit/consumerconsumecontext-2)<br/>

`next` [IPipe\<ConsumerConsumeContext\<TConsumer, TJob\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
