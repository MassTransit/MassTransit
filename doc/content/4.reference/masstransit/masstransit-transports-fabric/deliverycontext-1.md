---

title: DeliveryContext<T>

---

# DeliveryContext\<T\>

Namespace: MassTransit.Transports.Fabric

```csharp
public interface DeliveryContext<T>
```

#### Type Parameters

`T`<br/>

## Properties

### **CancellationToken**

```csharp
public abstract CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **Message**

The package being delivered

```csharp
public abstract T Message { get; }
```

#### Property Value

T<br/>

### **RoutingKey**

Optional routing key, which is used by direct/topic exchanges

```csharp
public abstract string RoutingKey { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **EnqueueTime**

Optional enqueue time, which can be used to delay messages

```csharp
public abstract Nullable<DateTime> EnqueueTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ReceiverId**

If specified, targets a specific receiver in the message fabric

```csharp
public abstract Nullable<long> ReceiverId { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Methods

### **WasAlreadyDelivered(IMessageSink\<T\>)**

Should this delivery occur, or has is already been delivered

```csharp
bool WasAlreadyDelivered(IMessageSink<T> sink)
```

#### Parameters

`sink` [IMessageSink\<T\>](../masstransit-transports-fabric/imessagesink-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Delivered(IMessageSink\<T\>)**

Marks the sink as delivered for this dispatch

```csharp
void Delivered(IMessageSink<T> sink)
```

#### Parameters

`sink` [IMessageSink\<T\>](../masstransit-transports-fabric/imessagesink-1)<br/>
