---

title: DeliveryMetrics

---

# DeliveryMetrics

Namespace: MassTransit.Transports

```csharp
public interface DeliveryMetrics
```

## Properties

### **DeliveryCount**

The number of messages consumed by the consumer

```csharp
public abstract long DeliveryCount { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **ConcurrentDeliveryCount**

The highest concurrent message count that was received by the consumer

```csharp
public abstract int ConcurrentDeliveryCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
