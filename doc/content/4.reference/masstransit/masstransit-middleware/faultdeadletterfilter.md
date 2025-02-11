---

title: FaultDeadLetterFilter

---

# FaultDeadLetterFilter

Namespace: MassTransit.Middleware

```csharp
public class FaultDeadLetterFilter : IFilter<ReceiveContext>, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FaultDeadLetterFilter](../masstransit-middleware/faultdeadletterfilter)<br/>
Implements [IFilter\<ReceiveContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **FaultDeadLetterFilter()**

```csharp
public FaultDeadLetterFilter()
```

## Methods

### **Send(ReceiveContext, IPipe\<ReceiveContext\>)**

```csharp
public Task Send(ReceiveContext context, IPipe<ReceiveContext> next)
```

#### Parameters

`context` [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

`next` [IPipe\<ReceiveContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
