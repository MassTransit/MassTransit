---

title: ISqlQueueEndpointConfigurator

---

# ISqlQueueEndpointConfigurator

Namespace: MassTransit

```csharp
public interface ISqlQueueEndpointConfigurator : ISqlQueueConfigurator
```

Implements [ISqlQueueConfigurator](../masstransit/isqlqueueconfigurator)

## Properties

### **PollingInterval**

The polling interval of the queue when notifications are not available (or trusted)

```csharp
public abstract TimeSpan PollingInterval { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **LockDuration**

The message lock duration (set higher for longer-running consumers)

```csharp
public abstract TimeSpan LockDuration { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **MaxLockDuration**

The maximum time a message can remain locked before being released for redelivery by the transport (up to MaxDeliveryCount)

```csharp
public abstract TimeSpan MaxLockDuration { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **PurgeOnStartup**

If true, messages that exist in the queue will be purged when the bus is started

```csharp
public abstract bool PurgeOnStartup { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **MaintenanceBatchSize**

Set the number of rows to process at a time when performing queue maintenance

```csharp
public abstract int MaintenanceBatchSize { set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **DeadLetterExpiredMessages**

If true, expired messages will be moved to the dead letter queue instead of deleted

```csharp
public abstract bool DeadLetterExpiredMessages { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
