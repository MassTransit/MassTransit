---

title: TopicSubscriptionPublishTopologySpecification

---

# TopicSubscriptionPublishTopologySpecification

Namespace: MassTransit.SqlTransport.Configuration

Used to bind an exchange to the sending

```csharp
public class TopicSubscriptionPublishTopologySpecification : SqlTopicSubscriptionConfigurator, ISqlTopicConfigurator, Topic, ISqlTopicSubscriptionConfigurator, ISqlPublishTopologySpecification, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlTopicConfigurator](../masstransit-sqltransport-configuration/sqltopicconfigurator) → [SqlTopicSubscriptionConfigurator](../masstransit-sqltransport-configuration/sqltopicsubscriptionconfigurator) → [TopicSubscriptionPublishTopologySpecification](../masstransit-sqltransport-configuration/topicsubscriptionpublishtopologyspecification)<br/>
Implements [ISqlTopicConfigurator](../masstransit/isqltopicconfigurator), [Topic](../masstransit-sqltransport-topology/topic), [ISqlTopicSubscriptionConfigurator](../masstransit/isqltopicsubscriptionconfigurator), [ISqlPublishTopologySpecification](../masstransit-sqltransport-configuration/isqlpublishtopologyspecification), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

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

### **TopicSubscriptionPublishTopologySpecification(String, SqlSubscriptionType, String)**

```csharp
public TopicSubscriptionPublishTopologySpecification(string topicName, SqlSubscriptionType subscriptionType, string routingKey)
```

#### Parameters

`topicName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`subscriptionType` [SqlSubscriptionType](../masstransit/sqlsubscriptiontype)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Apply(IPublishEndpointBrokerTopologyBuilder)**

```csharp
public void Apply(IPublishEndpointBrokerTopologyBuilder builder)
```

#### Parameters

`builder` [IPublishEndpointBrokerTopologyBuilder](../masstransit-sqltransport-topology/ipublishendpointbrokertopologybuilder)<br/>
