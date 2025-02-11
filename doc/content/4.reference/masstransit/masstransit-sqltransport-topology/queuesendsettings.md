---

title: QueueSendSettings

---

# QueueSendSettings

Namespace: MassTransit.SqlTransport.Topology

```csharp
public class QueueSendSettings : SqlQueueConfigurator, ISqlQueueConfigurator, Queue, SendSettings, EntitySettings
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlQueueConfigurator](../masstransit-sqltransport-configuration/sqlqueueconfigurator) → [QueueSendSettings](../masstransit-sqltransport-topology/queuesendsettings)<br/>
Implements [ISqlQueueConfigurator](../masstransit/isqlqueueconfigurator), [Queue](../masstransit-sqltransport-topology/queue), [SendSettings](../masstransit-sqltransport/sendsettings), [EntitySettings](../masstransit-sqltransport/entitysettings)

## Properties

### **EntityName**

```csharp
public string EntityName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

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

### **QueueSendSettings(SqlEndpointAddress)**

```csharp
public QueueSendSettings(SqlEndpointAddress address)
```

#### Parameters

`address` [SqlEndpointAddress](../masstransit/sqlendpointaddress)<br/>

### **QueueSendSettings(EntitySettings, String)**

```csharp
public QueueSendSettings(EntitySettings settings, string queueName)
```

#### Parameters

`settings` [EntitySettings](../masstransit-sqltransport/entitysettings)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **GetSendAddress(Uri)**

```csharp
public SqlEndpointAddress GetSendAddress(Uri hostAddress)
```

#### Parameters

`hostAddress` Uri<br/>

#### Returns

[SqlEndpointAddress](../masstransit/sqlendpointaddress)<br/>

### **GetBrokerTopology()**

```csharp
public BrokerTopology GetBrokerTopology()
```

#### Returns

[BrokerTopology](../masstransit-sqltransport-topology/brokertopology)<br/>
