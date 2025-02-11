---

title: FutureRequestPipe<T>

---

# FutureRequestPipe\<T\>

Namespace: MassTransit.Middleware

```csharp
public class FutureRequestPipe<T> : IPipe<SendContext<T>>, IProbeSite
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureRequestPipe\<T\>](../masstransit-middleware/futurerequestpipe-1)<br/>
Implements [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **FutureRequestPipe(IPipe\<SendContext\<T\>\>, Uri, Guid)**

```csharp
public FutureRequestPipe(IPipe<SendContext<T>> pipe, Uri responseAddress, Guid requestId)
```

#### Parameters

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`responseAddress` Uri<br/>

`requestId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

## Methods

### **Send(SendContext\<T\>)**

```csharp
public Task Send(SendContext<T> context)
```

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
