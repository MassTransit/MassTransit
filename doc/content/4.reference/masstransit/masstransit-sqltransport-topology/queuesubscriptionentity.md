---

title: QueueSubscriptionEntity

---

# QueueSubscriptionEntity

Namespace: MassTransit.SqlTransport.Topology

```csharp
public class QueueSubscriptionEntity : TopicToQueueSubscription, QueueSubscriptionHandle, EntityHandle
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [QueueSubscriptionEntity](../masstransit-sqltransport-topology/queuesubscriptionentity)<br/>
Implements [TopicToQueueSubscription](../masstransit-sqltransport-topology/topictoqueuesubscription), [QueueSubscriptionHandle](../masstransit-sqltransport-topology/queuesubscriptionhandle), [EntityHandle](../masstransit-topology/entityhandle)

## Properties

### **EntityComparer**

```csharp
public static IEqualityComparer<QueueSubscriptionEntity> EntityComparer { get; }
```

#### Property Value

[IEqualityComparer\<QueueSubscriptionEntity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)<br/>

### **Id**

```csharp
public long Id { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **Subscription**

```csharp
public TopicToQueueSubscription Subscription { get; }
```

#### Property Value

[TopicToQueueSubscription](../masstransit-sqltransport-topology/topictoqueuesubscription)<br/>

### **SubscriptionType**

```csharp
public SqlSubscriptionType SubscriptionType { get; }
```

#### Property Value

[SqlSubscriptionType](../masstransit/sqlsubscriptiontype)<br/>

### **Source**

```csharp
public Topic Source { get; }
```

#### Property Value

[Topic](../masstransit-sqltransport-topology/topic)<br/>

### **Destination**

```csharp
public Queue Destination { get; }
```

#### Property Value

[Queue](../masstransit-sqltransport-topology/queue)<br/>

### **RoutingKey**

```csharp
public string RoutingKey { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **QueueSubscriptionEntity(Int64, TopicEntity, QueueEntity, SqlSubscriptionType, String)**

```csharp
public QueueSubscriptionEntity(long id, TopicEntity topic, QueueEntity queue, SqlSubscriptionType subscriptionType, string routingKey)
```

#### Parameters

`id` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`topic` [TopicEntity](../masstransit-sqltransport-topology/topicentity)<br/>

`queue` [QueueEntity](../masstransit-sqltransport-topology/queueentity)<br/>

`subscriptionType` [SqlSubscriptionType](../masstransit/sqlsubscriptiontype)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
