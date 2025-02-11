---

title: SqlMessageContext

---

# SqlMessageContext

Namespace: MassTransit

```csharp
public interface SqlMessageContext : RoutingKeyConsumeContext, PartitionKeyConsumeContext
```

Implements [RoutingKeyConsumeContext](../../masstransit-abstractions/masstransit/routingkeyconsumecontext), [PartitionKeyConsumeContext](../../masstransit-abstractions/masstransit/partitionkeyconsumecontext)

## Properties

### **TransportMessage**

```csharp
public abstract SqlTransportMessage TransportMessage { get; }
```

#### Property Value

[SqlTransportMessage](../masstransit-sqltransport/sqltransportmessage)<br/>

### **TransportMessageId**

```csharp
public abstract Guid TransportMessageId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **DeliveryMessageId**

```csharp
public abstract long DeliveryMessageId { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **QueueName**

```csharp
public abstract string QueueName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ConsumerId**

```csharp
public abstract Nullable<Guid> ConsumerId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **LockId**

```csharp
public abstract Nullable<Guid> LockId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Priority**

```csharp
public abstract short Priority { get; }
```

#### Property Value

[Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

### **EnqueueTime**

```csharp
public abstract DateTime EnqueueTime { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **DeliveryCount**

```csharp
public abstract int DeliveryCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
