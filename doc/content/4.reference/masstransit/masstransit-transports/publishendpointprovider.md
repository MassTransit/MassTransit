---

title: PublishEndpointProvider

---

# PublishEndpointProvider

Namespace: MassTransit.Transports

```csharp
public class PublishEndpointProvider : IPublishEndpointProvider, IPublishObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishEndpointProvider](../masstransit-transports/publishendpointprovider)<br/>
Implements [IPublishEndpointProvider](../../masstransit-abstractions/masstransit/ipublishendpointprovider), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector)

## Constructors

### **PublishEndpointProvider(IPublishTransportProvider, Uri, PublishObservable, ReceiveEndpointContext, IPublishPipe, IPublishTopology)**

```csharp
public PublishEndpointProvider(IPublishTransportProvider transportProvider, Uri hostAddress, PublishObservable publishObservers, ReceiveEndpointContext context, IPublishPipe publishPipe, IPublishTopology publishTopology)
```

#### Parameters

`transportProvider` [IPublishTransportProvider](../../masstransit-abstractions/masstransit-transports/ipublishtransportprovider)<br/>

`hostAddress` Uri<br/>

`publishObservers` [PublishObservable](../../masstransit-abstractions/masstransit-observables/publishobservable)<br/>

`context` [ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>

`publishPipe` [IPublishPipe](../../masstransit-abstractions/masstransit-transports/ipublishpipe)<br/>

`publishTopology` [IPublishTopology](../../masstransit-abstractions/masstransit/ipublishtopology)<br/>

## Methods

### **GetPublishSendEndpoint\<T\>()**

```csharp
public Task<ISendEndpoint> GetPublishSendEndpoint<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ConnectPublishObserver(IPublishObserver)**

```csharp
public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
```

#### Parameters

`observer` [IPublishObserver](../../masstransit-abstractions/masstransit/ipublishobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
