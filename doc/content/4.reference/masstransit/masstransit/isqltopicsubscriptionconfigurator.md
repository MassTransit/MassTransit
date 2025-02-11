---

title: ISqlTopicSubscriptionConfigurator

---

# ISqlTopicSubscriptionConfigurator

Namespace: MassTransit

Configures the topic subscription for the receive endpoint

```csharp
public interface ISqlTopicSubscriptionConfigurator : ISqlTopicConfigurator
```

Implements [ISqlTopicConfigurator](../masstransit/isqltopicconfigurator)

## Properties

### **SubscriptionType**

```csharp
public abstract SqlSubscriptionType SubscriptionType { set; }
```

#### Property Value

[SqlSubscriptionType](../masstransit/sqlsubscriptiontype)<br/>

### **RoutingKey**

```csharp
public abstract string RoutingKey { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
