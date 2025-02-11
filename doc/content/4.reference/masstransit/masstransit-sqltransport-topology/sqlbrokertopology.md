---

title: SqlBrokerTopology

---

# SqlBrokerTopology

Namespace: MassTransit.SqlTransport.Topology

```csharp
public class SqlBrokerTopology : BrokerTopology, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlBrokerTopology](../masstransit-sqltransport-topology/sqlbrokertopology)<br/>
Implements [BrokerTopology](../masstransit-sqltransport-topology/brokertopology), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **Topics**

```csharp
public Topic[] Topics { get; }
```

#### Property Value

[Topic[]](../masstransit-sqltransport-topology/topic)<br/>

### **Queues**

```csharp
public Queue[] Queues { get; }
```

#### Property Value

[Queue[]](../masstransit-sqltransport-topology/queue)<br/>

### **TopicSubscriptions**

```csharp
public TopicToTopicSubscription[] TopicSubscriptions { get; }
```

#### Property Value

[TopicToTopicSubscription[]](../masstransit-sqltransport-topology/topictotopicsubscription)<br/>

### **QueueSubscriptions**

```csharp
public TopicToQueueSubscription[] QueueSubscriptions { get; }
```

#### Property Value

[TopicToQueueSubscription[]](../masstransit-sqltransport-topology/topictoqueuesubscription)<br/>

## Constructors

### **SqlBrokerTopology(IEnumerable\<Topic\>, IEnumerable\<TopicToTopicSubscription\>, IEnumerable\<Queue\>, IEnumerable\<TopicToQueueSubscription\>)**

```csharp
public SqlBrokerTopology(IEnumerable<Topic> topics, IEnumerable<TopicToTopicSubscription> topicSubscriptions, IEnumerable<Queue> queues, IEnumerable<TopicToQueueSubscription> queueSubscriptions)
```

#### Parameters

`topics` [IEnumerable\<Topic\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`topicSubscriptions` [IEnumerable\<TopicToTopicSubscription\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`queues` [IEnumerable\<Queue\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`queueSubscriptions` [IEnumerable\<TopicToQueueSubscription\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
