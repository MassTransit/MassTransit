---

title: QueueSubscriptionConsumeTopologySpecification

---

# QueueSubscriptionConsumeTopologySpecification

Namespace: MassTransit.SqlTransport.Configuration

```csharp
public class QueueSubscriptionConsumeTopologySpecification : SqlTopicSubscriptionConfigurator, ISqlTopicConfigurator, Topic, ISqlTopicSubscriptionConfigurator, ISqlConsumeTopologySpecification, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlTopicConfigurator](../masstransit-sqltransport-configuration/sqltopicconfigurator) → [SqlTopicSubscriptionConfigurator](../masstransit-sqltransport-configuration/sqltopicsubscriptionconfigurator) → [QueueSubscriptionConsumeTopologySpecification](../masstransit-sqltransport-configuration/queuesubscriptionconsumetopologyspecification)<br/>
Implements [ISqlTopicConfigurator](../masstransit/isqltopicconfigurator), [Topic](../masstransit-sqltransport-topology/topic), [ISqlTopicSubscriptionConfigurator](../masstransit/isqltopicsubscriptionconfigurator), [ISqlConsumeTopologySpecification](../masstransit-sqltransport-configuration/isqlconsumetopologyspecification), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

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

## Constructors

### **QueueSubscriptionConsumeTopologySpecification(String, SqlSubscriptionType, String)**

```csharp
public QueueSubscriptionConsumeTopologySpecification(string topicName, SqlSubscriptionType subscriptionType, string routingKey)
```

#### Parameters

`topicName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`subscriptionType` [SqlSubscriptionType](../masstransit/sqlsubscriptiontype)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **QueueSubscriptionConsumeTopologySpecification(Topic, SqlSubscriptionType, String)**

```csharp
public QueueSubscriptionConsumeTopologySpecification(Topic topic, SqlSubscriptionType subscriptionType, string routingKey)
```

#### Parameters

`topic` [Topic](../masstransit-sqltransport-topology/topic)<br/>

`subscriptionType` [SqlSubscriptionType](../masstransit/sqlsubscriptiontype)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Apply(IReceiveEndpointBrokerTopologyBuilder)**

```csharp
public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
```

#### Parameters

`builder` [IReceiveEndpointBrokerTopologyBuilder](../masstransit-sqltransport-topology/ireceiveendpointbrokertopologybuilder)<br/>
