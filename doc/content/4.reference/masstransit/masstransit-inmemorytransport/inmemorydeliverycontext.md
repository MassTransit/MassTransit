---

title: InMemoryDeliveryContext

---

# InMemoryDeliveryContext

Namespace: MassTransit.InMemoryTransport

```csharp
public class InMemoryDeliveryContext : DeliveryContext<InMemoryTransportMessage>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryDeliveryContext](../masstransit-inmemorytransport/inmemorydeliverycontext)<br/>
Implements [DeliveryContext\<InMemoryTransportMessage\>](../masstransit-transports-fabric/deliverycontext-1)

## Properties

### **CancellationToken**

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **Message**

```csharp
public InMemoryTransportMessage Message { get; }
```

#### Property Value

[InMemoryTransportMessage](../masstransit-inmemorytransport/inmemorytransportmessage)<br/>

### **RoutingKey**

```csharp
public string RoutingKey { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **EnqueueTime**

```csharp
public Nullable<DateTime> EnqueueTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ReceiverId**

```csharp
public Nullable<long> ReceiverId { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **InMemoryDeliveryContext(InMemoryTransportMessage, CancellationToken)**

```csharp
public InMemoryDeliveryContext(InMemoryTransportMessage message, CancellationToken cancellationToken)
```

#### Parameters

`message` [InMemoryTransportMessage](../masstransit-inmemorytransport/inmemorytransportmessage)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **WasAlreadyDelivered(IMessageSink\<InMemoryTransportMessage\>)**

```csharp
public bool WasAlreadyDelivered(IMessageSink<InMemoryTransportMessage> sink)
```

#### Parameters

`sink` [IMessageSink\<InMemoryTransportMessage\>](../masstransit-transports-fabric/imessagesink-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Delivered(IMessageSink\<InMemoryTransportMessage\>)**

```csharp
public void Delivered(IMessageSink<InMemoryTransportMessage> sink)
```

#### Parameters

`sink` [IMessageSink\<InMemoryTransportMessage\>](../masstransit-transports-fabric/imessagesink-1)<br/>
