---

title: MessageObserverConnector<TMessage>

---

# MessageObserverConnector\<TMessage\>

Namespace: MassTransit.Configuration

Connects a message handler to a pipe

```csharp
public class MessageObserverConnector<TMessage> : IObserverConnector<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageObserverConnector\<TMessage\>](../masstransit-configuration/messageobserverconnector-1)<br/>
Implements [IObserverConnector\<TMessage\>](../masstransit-configuration/iobserverconnector-1)

## Constructors

### **MessageObserverConnector()**

```csharp
public MessageObserverConnector()
```

## Methods

### **ConnectObserver(IConsumePipeConnector, IObserver\<ConsumeContext\<TMessage\>\>, IFilter`1[])**

```csharp
public ConnectHandle ConnectObserver(IConsumePipeConnector consumePipe, IObserver<ConsumeContext<TMessage>> observer, IFilter`1[] filters)
```

#### Parameters

`consumePipe` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`observer` [IObserver\<ConsumeContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.iobserver-1)<br/>

`filters` [IFilter`1[]](../../masstransit-abstractions/masstransit/ifilter-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectRequestObserver(IRequestPipeConnector, Guid, IObserver\<ConsumeContext\<TMessage\>\>, IFilter`1[])**

```csharp
public ConnectHandle ConnectRequestObserver(IRequestPipeConnector consumePipe, Guid requestId, IObserver<ConsumeContext<TMessage>> observer, IFilter`1[] filters)
```

#### Parameters

`consumePipe` [IRequestPipeConnector](../../masstransit-abstractions/masstransit/irequestpipeconnector)<br/>

`requestId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`observer` [IObserver\<ConsumeContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.iobserver-1)<br/>

`filters` [IFilter`1[]](../../masstransit-abstractions/masstransit/ifilter-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
