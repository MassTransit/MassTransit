---

title: TransportReadyFilter<T>

---

# TransportReadyFilter\<T\>

Namespace: MassTransit.Middleware

```csharp
public class TransportReadyFilter<T> : IFilter<T>, IProbeSite
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransportReadyFilter\<T\>](../masstransit-middleware/transportreadyfilter-1)<br/>
Implements [IFilter\<T\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **TransportReadyFilter(ReceiveEndpointContext)**

```csharp
public TransportReadyFilter(ReceiveEndpointContext context)
```

#### Parameters

`context` [ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>

## Methods

### **Send(T, IPipe\<T\>)**

```csharp
public Task Send(T context, IPipe<T> next)
```

#### Parameters

`context` T<br/>

`next` [IPipe\<T\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
