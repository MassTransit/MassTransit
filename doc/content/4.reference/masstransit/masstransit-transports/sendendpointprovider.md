---

title: SendEndpointProvider

---

# SendEndpointProvider

Namespace: MassTransit.Transports

```csharp
public class SendEndpointProvider : ISendEndpointProvider, ISendObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendEndpointProvider](../masstransit-transports/sendendpointprovider)<br/>
Implements [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector)

## Constructors

### **SendEndpointProvider(ISendTransportProvider, SendObservable, ReceiveEndpointContext, ISendPipe)**

```csharp
public SendEndpointProvider(ISendTransportProvider provider, SendObservable observers, ReceiveEndpointContext context, ISendPipe sendPipe)
```

#### Parameters

`provider` [ISendTransportProvider](../../masstransit-abstractions/masstransit-transports/isendtransportprovider)<br/>

`observers` [SendObservable](../../masstransit-abstractions/masstransit-observables/sendobservable)<br/>

`context` [ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>

`sendPipe` [ISendPipe](../../masstransit-abstractions/masstransit-transports/isendpipe)<br/>

## Methods

### **GetSendEndpoint(Uri)**

```csharp
public Task<ISendEndpoint> GetSendEndpoint(Uri address)
```

#### Parameters

`address` Uri<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ConnectSendObserver(ISendObserver)**

```csharp
public ConnectHandle ConnectSendObserver(ISendObserver observer)
```

#### Parameters

`observer` [ISendObserver](../../masstransit-abstractions/masstransit/isendobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
