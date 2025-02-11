---

title: MessageSchedulerFilter

---

# MessageSchedulerFilter

Namespace: MassTransit.Middleware

Adds the scheduler to the consume context, so that it can be used for message redelivery

```csharp
public class MessageSchedulerFilter : IFilter<ConsumeContext>, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageSchedulerFilter](../masstransit-middleware/messageschedulerfilter)<br/>
Implements [IFilter\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **MessageSchedulerFilter(Uri)**

```csharp
public MessageSchedulerFilter(Uri schedulerAddress)
```

#### Parameters

`schedulerAddress` Uri<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Send(ConsumeContext, IPipe\<ConsumeContext\>)**

```csharp
public Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
```

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`next` [IPipe\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
