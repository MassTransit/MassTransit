---

title: IInMemoryTransportProvider

---

# IInMemoryTransportProvider

Namespace: MassTransit.InMemoryTransport

```csharp
public interface IInMemoryTransportProvider : InMemoryTransportContext, IAgent, IProbeSite
```

Implements [InMemoryTransportContext](../masstransit-inmemorytransport/inmemorytransportcontext), [IAgent](../../masstransit-abstractions/masstransit/iagent), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **MessageFabric**

```csharp
public abstract IMessageFabric<InMemoryTransportContext, InMemoryTransportMessage> MessageFabric { get; }
```

#### Property Value

[IMessageFabric\<InMemoryTransportContext, InMemoryTransportMessage\>](../masstransit-transports-fabric/imessagefabric-2)<br/>

## Methods

### **CreateSendTransport(ReceiveEndpointContext, Uri)**

```csharp
Task<ISendTransport> CreateSendTransport(ReceiveEndpointContext context, Uri address)
```

#### Parameters

`context` [ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>

`address` Uri<br/>

#### Returns

[Task\<ISendTransport\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **CreatePublishTransport\<T\>(ReceiveEndpointContext, Uri)**

```csharp
Task<ISendTransport> CreatePublishTransport<T>(ReceiveEndpointContext context, Uri publishAddress)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>

`publishAddress` Uri<br/>

#### Returns

[Task\<ISendTransport\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **NormalizeAddress(Uri)**

```csharp
Uri NormalizeAddress(Uri address)
```

#### Parameters

`address` Uri<br/>

#### Returns

Uri<br/>
