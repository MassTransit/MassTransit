---

title: InMemorySendTransportProvider

---

# InMemorySendTransportProvider

Namespace: MassTransit.InMemoryTransport

```csharp
public class InMemorySendTransportProvider : ISendTransportProvider
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemorySendTransportProvider](../masstransit-inmemorytransport/inmemorysendtransportprovider)<br/>
Implements [ISendTransportProvider](../../masstransit-abstractions/masstransit-transports/isendtransportprovider)

## Constructors

### **InMemorySendTransportProvider(IInMemoryTransportProvider, ReceiveEndpointContext)**

```csharp
public InMemorySendTransportProvider(IInMemoryTransportProvider transportProvider, ReceiveEndpointContext context)
```

#### Parameters

`transportProvider` [IInMemoryTransportProvider](../masstransit-inmemorytransport/iinmemorytransportprovider)<br/>

`context` [ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>

## Methods

### **NormalizeAddress(Uri)**

```csharp
public Uri NormalizeAddress(Uri address)
```

#### Parameters

`address` Uri<br/>

#### Returns

Uri<br/>

### **GetSendTransport(Uri)**

```csharp
public Task<ISendTransport> GetSendTransport(Uri address)
```

#### Parameters

`address` Uri<br/>

#### Returns

[Task\<ISendTransport\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
