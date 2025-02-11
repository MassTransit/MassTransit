---

title: SqlQueueSubscriptionConfigurator

---

# SqlQueueSubscriptionConfigurator

Namespace: MassTransit.SqlTransport.Configuration

```csharp
public class SqlQueueSubscriptionConfigurator : SqlTopicSubscriptionConfigurator, ISqlTopicConfigurator, Topic, ISqlTopicSubscriptionConfigurator
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlTopicConfigurator](../masstransit-sqltransport-configuration/sqltopicconfigurator) → [SqlTopicSubscriptionConfigurator](../masstransit-sqltransport-configuration/sqltopicsubscriptionconfigurator) → [SqlQueueSubscriptionConfigurator](../masstransit-sqltransport-configuration/sqlqueuesubscriptionconfigurator)<br/>
Implements [ISqlTopicConfigurator](../masstransit/isqltopicconfigurator), [Topic](../masstransit-sqltransport-topology/topic), [ISqlTopicSubscriptionConfigurator](../masstransit/isqltopicsubscriptionconfigurator)

## Properties

### **RoutingKey**

```csharp
public string RoutingKey { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **SubscriptionType**

```csharp
public SqlSubscriptionType SubscriptionType { get; set; }
```

#### Property Value

[SqlSubscriptionType](../masstransit/sqlsubscriptiontype)<br/>

### **TopicName**

```csharp
public string TopicName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
