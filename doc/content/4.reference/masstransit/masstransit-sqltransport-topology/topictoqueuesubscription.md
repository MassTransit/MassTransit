---

title: TopicToQueueSubscription

---

# TopicToQueueSubscription

Namespace: MassTransit.SqlTransport.Topology

The exchange to queue binding details to declare the binding to RabbitMQ

```csharp
public interface TopicToQueueSubscription
```

## Properties

### **Source**

The source exchange

```csharp
public abstract Topic Source { get; }
```

#### Property Value

[Topic](../masstransit-sqltransport-topology/topic)<br/>

### **Destination**

The destination exchange

```csharp
public abstract Queue Destination { get; }
```

#### Property Value

[Queue](../masstransit-sqltransport-topology/queue)<br/>

### **SubscriptionType**

```csharp
public abstract SqlSubscriptionType SubscriptionType { get; }
```

#### Property Value

[SqlSubscriptionType](../masstransit/sqlsubscriptiontype)<br/>

### **RoutingKey**

A routing key for the exchange binding

```csharp
public abstract string RoutingKey { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
