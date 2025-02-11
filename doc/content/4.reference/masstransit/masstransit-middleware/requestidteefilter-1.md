---

title: RequestIdTeeFilter<TMessage>

---

# RequestIdTeeFilter\<TMessage\>

Namespace: MassTransit.Middleware

```csharp
public class RequestIdTeeFilter<TMessage> : TeeFilter<ConsumeContext<TMessage>>, ITeeFilter<ConsumeContext<TMessage>>, IFilter<ConsumeContext<TMessage>>, IProbeSite, IPipeConnector<ConsumeContext<TMessage>>, IRequestIdTeeFilter<TMessage>, IKeyPipeConnector<TMessage, Guid>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TeeFilter\<ConsumeContext\<TMessage\>\>](../masstransit-middleware/teefilter-1) → [RequestIdTeeFilter\<TMessage\>](../masstransit-middleware/requestidteefilter-1)<br/>
Implements [ITeeFilter\<ConsumeContext\<TMessage\>\>](../masstransit-middleware/iteefilter-1), [IFilter\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IPipeConnector\<ConsumeContext\<TMessage\>\>](../masstransit-middleware/ipipeconnector-1), [IRequestIdTeeFilter\<TMessage\>](../masstransit-middleware/irequestidteefilter-1), [IKeyPipeConnector\<TMessage, Guid\>](../masstransit-middleware/ikeypipeconnector-2)

## Properties

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **RequestIdTeeFilter()**

```csharp
public RequestIdTeeFilter()
```

## Methods

### **ConnectPipe(Guid, IPipe\<ConsumeContext\<TMessage\>\>)**

```csharp
public ConnectHandle ConnectPipe(Guid key, IPipe<ConsumeContext<TMessage>> pipe)
```

#### Parameters

`key` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`pipe` [IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
