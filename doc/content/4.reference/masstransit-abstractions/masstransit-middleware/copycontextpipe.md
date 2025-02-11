---

title: CopyContextPipe

---

# CopyContextPipe

Namespace: MassTransit.Middleware

```csharp
public class CopyContextPipe : IPipe<SendContext>, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CopyContextPipe](../masstransit-middleware/copycontextpipe)<br/>
Implements [IPipe\<SendContext\>](../masstransit/ipipe-1), [IProbeSite](../masstransit/iprobesite)

## Constructors

### **CopyContextPipe(ConsumeContext, Action\<ConsumeContext, SendContext\>)**

```csharp
public CopyContextPipe(ConsumeContext context, Action<ConsumeContext, SendContext> callback)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`callback` [Action\<ConsumeContext, SendContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

## Methods

### **Send(SendContext)**

```csharp
public Task Send(SendContext context)
```

#### Parameters

`context` [SendContext](../masstransit/sendcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../masstransit/probecontext)<br/>
