---

title: QueueMetric

---

# QueueMetric

Namespace: MassTransit.Transports.Fabric

```csharp
public class QueueMetric
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [QueueMetric](../masstransit-transports-fabric/queuemetric)

## Properties

### **Name**

```csharp
public string Name { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **DeliveryCount**

Total number of messages delivered

```csharp
public Counter DeliveryCount { get; }
```

#### Property Value

[Counter](../masstransit-transports-fabric/counter)<br/>

### **ActiveDeliveryCount**

Number of messages currently being delivered

```csharp
public Gauge ActiveDeliveryCount { get; }
```

#### Property Value

[Gauge](../masstransit-transports-fabric/gauge)<br/>

### **DelayedMessageCount**

Number of messages currently delayed before entering the queue

```csharp
public Gauge DelayedMessageCount { get; }
```

#### Property Value

[Gauge](../masstransit-transports-fabric/gauge)<br/>

### **MessageCount**

Number of messages currently in the queue (not including active messages)

```csharp
public Gauge MessageCount { get; }
```

#### Property Value

[Gauge](../masstransit-transports-fabric/gauge)<br/>

## Constructors

### **QueueMetric(String)**

```csharp
public QueueMetric(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
