---

title: RequestIdFilter<TMessage>

---

# RequestIdFilter\<TMessage\>

Namespace: MassTransit.Middleware

Handles the registration of requests and connecting them to the consume pipe

```csharp
public class RequestIdFilter<TMessage> : IFilter<ConsumeContext<TMessage>>, IProbeSite, IKeyPipeConnector<TMessage, Guid>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestIdFilter\<TMessage\>](../masstransit-middleware/requestidfilter-1)<br/>
Implements [IFilter\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IKeyPipeConnector\<TMessage, Guid\>](../masstransit-middleware/ikeypipeconnector-2)

## Constructors

### **RequestIdFilter()**

```csharp
public RequestIdFilter()
```

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Send(ConsumeContext\<TMessage\>, IPipe\<ConsumeContext\<TMessage\>\>)**

```csharp
public Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`next` [IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ConnectPipe(Guid, IPipe\<ConsumeContext\<TMessage\>\>)**

```csharp
public ConnectHandle ConnectPipe(Guid key, IPipe<ConsumeContext<TMessage>> pipe)
```

#### Parameters

`key` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`pipe` [IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
