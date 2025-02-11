---

title: InMemoryPublishTransportProvider

---

# InMemoryPublishTransportProvider

Namespace: MassTransit.InMemoryTransport

```csharp
public class InMemoryPublishTransportProvider : IPublishTransportProvider
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryPublishTransportProvider](../masstransit-inmemorytransport/inmemorypublishtransportprovider)<br/>
Implements [IPublishTransportProvider](../../masstransit-abstractions/masstransit-transports/ipublishtransportprovider)

## Constructors

### **InMemoryPublishTransportProvider(IInMemoryTransportProvider, ReceiveEndpointContext)**

```csharp
public InMemoryPublishTransportProvider(IInMemoryTransportProvider transportProvider, ReceiveEndpointContext context)
```

#### Parameters

`transportProvider` [IInMemoryTransportProvider](../masstransit-inmemorytransport/iinmemorytransportprovider)<br/>

`context` [ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>

## Methods

### **GetPublishTransport\<T\>(Uri)**

```csharp
public Task<ISendTransport> GetPublishTransport<T>(Uri publishAddress)
```

#### Type Parameters

`T`<br/>

#### Parameters

`publishAddress` Uri<br/>

#### Returns

[Task\<ISendTransport\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
