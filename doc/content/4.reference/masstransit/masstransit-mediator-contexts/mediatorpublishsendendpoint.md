---

title: MediatorPublishSendEndpoint

---

# MediatorPublishSendEndpoint

Namespace: MassTransit.Mediator.Contexts



```csharp
public class MediatorPublishSendEndpoint : SendEndpointProxy, ITransportSendEndpoint, ISendEndpoint, ISendObserverConnector, IPublishObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendEndpointProxy](../masstransit-transports/sendendpointproxy) → [MediatorPublishSendEndpoint](../masstransit-mediator-contexts/mediatorpublishsendendpoint)<br/>
Implements [ITransportSendEndpoint](../masstransit-transports/itransportsendendpoint), [ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector)

## Properties

### **Endpoint**

```csharp
public ISendEndpoint Endpoint { get; }
```

#### Property Value

[ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint)<br/>

## Constructors

### **MediatorPublishSendEndpoint(ISendEndpoint, IPublishPipe)**

```csharp
public MediatorPublishSendEndpoint(ISendEndpoint endpoint, IPublishPipe publishPipe)
```

#### Parameters

`endpoint` [ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint)<br/>

`publishPipe` [IPublishPipe](../../masstransit-abstractions/masstransit-transports/ipublishpipe)<br/>

## Methods

### **ConnectPublishObserver(IPublishObserver)**

```csharp
public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
```

#### Parameters

`observer` [IPublishObserver](../../masstransit-abstractions/masstransit/ipublishobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **GetPipeProxy\<T\>(IPipe\<SendContext\<T\>\>)**

```csharp
protected IPipe<SendContext<T>> GetPipeProxy<T>(IPipe<SendContext<T>> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>
