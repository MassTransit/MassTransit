---

title: BrokerTopologyBuilder

---

# BrokerTopologyBuilder

Namespace: MassTransit.SqlTransport.Topology

```csharp
public abstract class BrokerTopologyBuilder
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BrokerTopologyBuilder](../masstransit-sqltransport-topology/brokertopologybuilder)

## Methods

### **CreateTopic(String)**

```csharp
public TopicHandle CreateTopic(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[TopicHandle](../masstransit-sqltransport-topology/topichandle)<br/>

### **CreateTopicSubscription(TopicHandle, TopicHandle, SqlSubscriptionType, String)**

```csharp
public TopicSubscriptionHandle CreateTopicSubscription(TopicHandle source, TopicHandle destination, SqlSubscriptionType subscriptionType, string routingKey)
```

#### Parameters

`source` [TopicHandle](../masstransit-sqltransport-topology/topichandle)<br/>

`destination` [TopicHandle](../masstransit-sqltransport-topology/topichandle)<br/>

`subscriptionType` [SqlSubscriptionType](../masstransit/sqlsubscriptiontype)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[TopicSubscriptionHandle](../masstransit-sqltransport-topology/topicsubscriptionhandle)<br/>

### **CreateQueue(String, Nullable\<TimeSpan\>, Nullable\<Int32\>)**

```csharp
public QueueHandle CreateQueue(string name, Nullable<TimeSpan> autoDeleteOnIdle, Nullable<int> maxDeliveryCount)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`autoDeleteOnIdle` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`maxDeliveryCount` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[QueueHandle](../masstransit-sqltransport-topology/queuehandle)<br/>

### **CreateQueueSubscription(TopicHandle, QueueHandle, SqlSubscriptionType, String)**

```csharp
public QueueSubscriptionHandle CreateQueueSubscription(TopicHandle topic, QueueHandle queue, SqlSubscriptionType subscriptionType, string routingKey)
```

#### Parameters

`topic` [TopicHandle](../masstransit-sqltransport-topology/topichandle)<br/>

`queue` [QueueHandle](../masstransit-sqltransport-topology/queuehandle)<br/>

`subscriptionType` [SqlSubscriptionType](../masstransit/sqlsubscriptiontype)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[QueueSubscriptionHandle](../masstransit-sqltransport-topology/queuesubscriptionhandle)<br/>

### **BuildBrokerTopology()**

```csharp
public BrokerTopology BuildBrokerTopology()
```

#### Returns

[BrokerTopology](../masstransit-sqltransport-topology/brokertopology)<br/>
