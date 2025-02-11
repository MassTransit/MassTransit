---

title: IObserverConnector<TMessage>

---

# IObserverConnector\<TMessage\>

Namespace: MassTransit.Configuration

Connects a message handler to the ConsumePipe

```csharp
public interface IObserverConnector<TMessage>
```

#### Type Parameters

`TMessage`<br/>
The message type

## Methods

### **ConnectObserver(IConsumePipeConnector, IObserver\<ConsumeContext\<TMessage\>\>, IFilter`1[])**

Connect a message handler for all messages of type T

```csharp
ConnectHandle ConnectObserver(IConsumePipeConnector consumePipe, IObserver<ConsumeContext<TMessage>> observer, IFilter`1[] filters)
```

#### Parameters

`consumePipe` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`observer` [IObserver\<ConsumeContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.iobserver-1)<br/>

`filters` [IFilter`1[]](../../masstransit-abstractions/masstransit/ifilter-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectRequestObserver(IRequestPipeConnector, Guid, IObserver\<ConsumeContext\<TMessage\>\>, IFilter`1[])**

Connect a message handler for messages with the specified RequestId

```csharp
ConnectHandle ConnectRequestObserver(IRequestPipeConnector consumePipe, Guid requestId, IObserver<ConsumeContext<TMessage>> observer, IFilter`1[] filters)
```

#### Parameters

`consumePipe` [IRequestPipeConnector](../../masstransit-abstractions/masstransit/irequestpipeconnector)<br/>

`requestId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`observer` [IObserver\<ConsumeContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.iobserver-1)<br/>

`filters` [IFilter`1[]](../../masstransit-abstractions/masstransit/ifilter-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
