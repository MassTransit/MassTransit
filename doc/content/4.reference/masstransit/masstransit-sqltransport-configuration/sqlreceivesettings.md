---

title: SqlReceiveSettings

---

# SqlReceiveSettings

Namespace: MassTransit.SqlTransport.Configuration

```csharp
public class SqlReceiveSettings : SqlQueueConfigurator, ISqlQueueConfigurator, Queue, ReceiveSettings, EntitySettings
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlQueueConfigurator](../masstransit-sqltransport-configuration/sqlqueueconfigurator) → [SqlReceiveSettings](../masstransit-sqltransport-configuration/sqlreceivesettings)<br/>
Implements [ISqlQueueConfigurator](../masstransit/isqlqueueconfigurator), [Queue](../masstransit-sqltransport-topology/queue), [ReceiveSettings](../masstransit-sqltransport/receivesettings), [EntitySettings](../masstransit-sqltransport/entitysettings)

## Properties

### **QueueId**

```csharp
public Nullable<long> QueueId { get; set; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **PrefetchCount**

```csharp
public int PrefetchCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ConcurrentMessageLimit**

```csharp
public int ConcurrentMessageLimit { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ConcurrentDeliveryLimit**

```csharp
public int ConcurrentDeliveryLimit { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ReceiveMode**

```csharp
public SqlReceiveMode ReceiveMode { get; set; }
```

#### Property Value

[SqlReceiveMode](../masstransit/sqlreceivemode)<br/>

### **PurgeOnStartup**

```csharp
public bool PurgeOnStartup { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **LockDuration**

```csharp
public TimeSpan LockDuration { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **PollingInterval**

```csharp
public TimeSpan PollingInterval { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **UnlockDelay**

```csharp
public Nullable<TimeSpan> UnlockDelay { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **MaxLockDuration**

```csharp
public TimeSpan MaxLockDuration { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **EntityName**

```csharp
public string EntityName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **MaintenanceBatchSize**

```csharp
public int MaintenanceBatchSize { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **DeadLetterExpiredMessages**

```csharp
public bool DeadLetterExpiredMessages { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **AutoDeleteOnIdle**

```csharp
public Nullable<TimeSpan> AutoDeleteOnIdle { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **MaxDeliveryCount**

```csharp
public Nullable<int> MaxDeliveryCount { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **QueueName**

```csharp
public string QueueName { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **SqlReceiveSettings(ISqlEndpointConfiguration, String, Nullable\<TimeSpan\>)**

```csharp
public SqlReceiveSettings(ISqlEndpointConfiguration configuration, string queueName, Nullable<TimeSpan> autoDeleteOnIdle)
```

#### Parameters

`configuration` [ISqlEndpointConfiguration](../masstransit-sqltransport-configuration/isqlendpointconfiguration)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`autoDeleteOnIdle` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Methods

### **GetInputAddress(Uri)**

```csharp
public Uri GetInputAddress(Uri hostAddress)
```

#### Parameters

`hostAddress` Uri<br/>

#### Returns

Uri<br/>
