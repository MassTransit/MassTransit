---

title: MessagePerformanceCounters

---

# MessagePerformanceCounters

Namespace: MassTransit.Monitoring.Performance

```csharp
public class MessagePerformanceCounters : BasePerformanceCounters
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BasePerformanceCounters](../masstransit-monitoring-performance/baseperformancecounters) → [MessagePerformanceCounters](../masstransit-monitoring-performance/messageperformancecounters)

## Properties

### **ConsumedPerSecond**

```csharp
public static CounterData ConsumedPerSecond { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

### **TotalReceived**

```csharp
public static CounterData TotalReceived { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

### **ConsumeDuration**

```csharp
public static CounterData ConsumeDuration { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

### **ConsumeDurationBase**

```csharp
public static CounterData ConsumeDurationBase { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

### **ConsumeFaulted**

```csharp
public static CounterData ConsumeFaulted { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

### **ConsumeFaultPercentage**

```csharp
public static CounterData ConsumeFaultPercentage { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

### **ConsumeFaultPercentageBase**

```csharp
public static CounterData ConsumeFaultPercentageBase { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

### **SentPerSecond**

```csharp
public static CounterData SentPerSecond { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

### **TotalSent**

```csharp
public static CounterData TotalSent { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

### **SendFaulted**

```csharp
public static CounterData SendFaulted { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

### **SendFaultPercentage**

```csharp
public static CounterData SendFaultPercentage { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

### **SendFaultPercentageBase**

```csharp
public static CounterData SendFaultPercentageBase { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

### **PublishedPerSecond**

```csharp
public static CounterData PublishedPerSecond { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

### **TotalPublished**

```csharp
public static CounterData TotalPublished { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

### **PublishFaulted**

```csharp
public static CounterData PublishFaulted { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

### **PublishFaultPercentage**

```csharp
public static CounterData PublishFaultPercentage { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

### **PublishFaultPercentageBase**

```csharp
public static CounterData PublishFaultPercentageBase { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

## Constructors

### **MessagePerformanceCounters()**

```csharp
public MessagePerformanceCounters()
```

## Methods

### **GetCounterData()**

```csharp
protected IEnumerable<CounterData> GetCounterData()
```

#### Returns

[IEnumerable\<CounterData\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Install()**

```csharp
public static void Install()
```
