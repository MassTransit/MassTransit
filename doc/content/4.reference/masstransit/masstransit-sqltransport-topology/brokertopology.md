---

title: BrokerTopology

---

# BrokerTopology

Namespace: MassTransit.SqlTransport.Topology

```csharp
public interface BrokerTopology : IProbeSite
```

Implements [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **Topics**

```csharp
public abstract Topic[] Topics { get; }
```

#### Property Value

[Topic[]](../masstransit-sqltransport-topology/topic)<br/>

### **Queues**

```csharp
public abstract Queue[] Queues { get; }
```

#### Property Value

[Queue[]](../masstransit-sqltransport-topology/queue)<br/>

### **TopicSubscriptions**

```csharp
public abstract TopicToTopicSubscription[] TopicSubscriptions { get; }
```

#### Property Value

[TopicToTopicSubscription[]](../masstransit-sqltransport-topology/topictotopicsubscription)<br/>

### **QueueSubscriptions**

```csharp
public abstract TopicToQueueSubscription[] QueueSubscriptions { get; }
```

#### Property Value

[TopicToQueueSubscription[]](../masstransit-sqltransport-topology/topictoqueuesubscription)<br/>
