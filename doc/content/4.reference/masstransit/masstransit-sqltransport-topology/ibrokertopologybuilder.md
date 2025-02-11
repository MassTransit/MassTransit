---

title: IBrokerTopologyBuilder

---

# IBrokerTopologyBuilder

Namespace: MassTransit.SqlTransport.Topology

```csharp
public interface IBrokerTopologyBuilder
```

## Methods

### **CreateTopic(String)**

Declares an exchange

```csharp
TopicHandle CreateTopic(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The exchange name

#### Returns

[TopicHandle](../masstransit-sqltransport-topology/topichandle)<br/>
An entity handle used to reference the exchange in subsequent calls

### **CreateTopicSubscription(TopicHandle, TopicHandle, SqlSubscriptionType, String)**

Bind an exchange to an exchange, with the specified routing key and arguments

```csharp
TopicSubscriptionHandle CreateTopicSubscription(TopicHandle source, TopicHandle destination, SqlSubscriptionType subscriptionType, string routingKey)
```

#### Parameters

`source` [TopicHandle](../masstransit-sqltransport-topology/topichandle)<br/>
The source exchange

`destination` [TopicHandle](../masstransit-sqltransport-topology/topichandle)<br/>
The destination exchange

`subscriptionType` [SqlSubscriptionType](../masstransit/sqlsubscriptiontype)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The binding routing key

#### Returns

[TopicSubscriptionHandle](../masstransit-sqltransport-topology/topicsubscriptionhandle)<br/>
An entity handle used to reference the binding in subsequent calls

### **CreateQueue(String, Nullable\<TimeSpan\>, Nullable\<Int32\>)**

Declares a queue

```csharp
QueueHandle CreateQueue(string name, Nullable<TimeSpan> autoDeleteOnIdle, Nullable<int> maxDeliveryCount)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`autoDeleteOnIdle` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`maxDeliveryCount` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[QueueHandle](../masstransit-sqltransport-topology/queuehandle)<br/>

### **CreateQueueSubscription(TopicHandle, QueueHandle, SqlSubscriptionType, String)**

Binds an exchange to a queue, with the specified routing key and arguments

```csharp
QueueSubscriptionHandle CreateQueueSubscription(TopicHandle topic, QueueHandle queue, SqlSubscriptionType subscriptionType, string routingKey)
```

#### Parameters

`topic` [TopicHandle](../masstransit-sqltransport-topology/topichandle)<br/>

`queue` [QueueHandle](../masstransit-sqltransport-topology/queuehandle)<br/>

`subscriptionType` [SqlSubscriptionType](../masstransit/sqlsubscriptiontype)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[QueueSubscriptionHandle](../masstransit-sqltransport-topology/queuesubscriptionhandle)<br/>
