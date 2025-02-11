---

title: ReceiveSettings

---

# ReceiveSettings

Namespace: MassTransit.SqlTransport

Specify the receive settings for a receive transport

```csharp
public interface ReceiveSettings : EntitySettings
```

Implements [EntitySettings](../masstransit-sqltransport/entitysettings)

## Properties

### **QueueName**

The queue name to receive from

```csharp
public abstract string QueueName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **QueueId**

Once the topology is configured, the queueId should be available

```csharp
public abstract Nullable<long> QueueId { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **PrefetchCount**

The number of unacknowledged messages to allow to be processed concurrently

```csharp
public abstract int PrefetchCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ConcurrentMessageLimit**

```csharp
public abstract int ConcurrentMessageLimit { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ConcurrentDeliveryLimit**

```csharp
public abstract int ConcurrentDeliveryLimit { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ReceiveMode**

```csharp
public abstract SqlReceiveMode ReceiveMode { get; }
```

#### Property Value

[SqlReceiveMode](../masstransit/sqlreceivemode)<br/>

### **PurgeOnStartup**

If True, and a queue name is specified, if the queue exists and has messages, they are purged at startup
 If the connection is reset, messages are not purged until the service is reset

```csharp
public abstract bool PurgeOnStartup { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **LockDuration**

Message locks are automatically renewed, however, the actual lock duration determines how long the message remains locked
 when the consumer process crashes.

```csharp
public abstract TimeSpan LockDuration { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **MaxLockDuration**

The maximum amount of time the lock will be renewed during message consumption before being abandoned

```csharp
public abstract TimeSpan MaxLockDuration { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **MaxDeliveryCount**

The maximum number of message delivery attempts by the transport before moving the message to the DLQ (defaults to 10)

```csharp
public abstract Nullable<int> MaxDeliveryCount { get; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **PollingInterval**

How often to poll for messages when no messages exist

```csharp
public abstract TimeSpan PollingInterval { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **UnlockDelay**

The amount of time, when a message is abandoned, before the message is available for redelivery

```csharp
public abstract Nullable<TimeSpan> UnlockDelay { get; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **MaintenanceBatchSize**

```csharp
public abstract int MaintenanceBatchSize { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **DeadLetterExpiredMessages**

```csharp
public abstract bool DeadLetterExpiredMessages { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
