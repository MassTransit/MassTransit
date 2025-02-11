---

title: TopicSubscriptionEntity

---

# TopicSubscriptionEntity

Namespace: MassTransit.SqlTransport.Topology

```csharp
public class TopicSubscriptionEntity : TopicToTopicSubscription, TopicSubscriptionHandle, EntityHandle
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TopicSubscriptionEntity](../masstransit-sqltransport-topology/topicsubscriptionentity)<br/>
Implements [TopicToTopicSubscription](../masstransit-sqltransport-topology/topictotopicsubscription), [TopicSubscriptionHandle](../masstransit-sqltransport-topology/topicsubscriptionhandle), [EntityHandle](../masstransit-topology/entityhandle)

## Properties

### **EntityComparer**

```csharp
public static IEqualityComparer<TopicSubscriptionEntity> EntityComparer { get; }
```

#### Property Value

[IEqualityComparer\<TopicSubscriptionEntity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)<br/>

### **Id**

```csharp
public long Id { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **Subscription**

```csharp
public TopicToTopicSubscription Subscription { get; }
```

#### Property Value

[TopicToTopicSubscription](../masstransit-sqltransport-topology/topictotopicsubscription)<br/>

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
public Topic Destination { get; }
```

#### Property Value

[Topic](../masstransit-sqltransport-topology/topic)<br/>

### **RoutingKey**

```csharp
public string RoutingKey { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **TopicSubscriptionEntity(Int64, TopicEntity, TopicEntity, SqlSubscriptionType, String)**

```csharp
public TopicSubscriptionEntity(long id, TopicEntity source, TopicEntity destination, SqlSubscriptionType subscriptionType, string routingKey)
```

#### Parameters

`id` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`source` [TopicEntity](../masstransit-sqltransport-topology/topicentity)<br/>

`destination` [TopicEntity](../masstransit-sqltransport-topology/topicentity)<br/>

`subscriptionType` [SqlSubscriptionType](../masstransit/sqlsubscriptiontype)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
