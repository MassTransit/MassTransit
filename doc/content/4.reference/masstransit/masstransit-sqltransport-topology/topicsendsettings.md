---

title: TopicSendSettings

---

# TopicSendSettings

Namespace: MassTransit.SqlTransport.Topology

```csharp
public class TopicSendSettings : SqlTopicConfigurator, ISqlTopicConfigurator, Topic, SendSettings, EntitySettings
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlTopicConfigurator](../masstransit-sqltransport-configuration/sqltopicconfigurator) → [TopicSendSettings](../masstransit-sqltransport-topology/topicsendsettings)<br/>
Implements [ISqlTopicConfigurator](../masstransit/isqltopicconfigurator), [Topic](../masstransit-sqltransport-topology/topic), [SendSettings](../masstransit-sqltransport/sendsettings), [EntitySettings](../masstransit-sqltransport/entitysettings)

## Properties

### **EntityName**

```csharp
public string EntityName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **AutoDeleteOnIdle**

```csharp
public Nullable<TimeSpan> AutoDeleteOnIdle { get; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **TopicName**

```csharp
public string TopicName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **TopicSendSettings(SqlEndpointAddress)**

```csharp
public TopicSendSettings(SqlEndpointAddress address)
```

#### Parameters

`address` [SqlEndpointAddress](../masstransit/sqlendpointaddress)<br/>

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
