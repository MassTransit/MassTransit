---

title: ConsumerPerformanceCounters

---

# ConsumerPerformanceCounters

Namespace: MassTransit.Monitoring.Performance

```csharp
public class ConsumerPerformanceCounters : BasePerformanceCounters
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BasePerformanceCounters](../masstransit-monitoring-performance/baseperformancecounters) → [ConsumerPerformanceCounters](../masstransit-monitoring-performance/consumerperformancecounters)

## Properties

### **ConsumeRate**

```csharp
public static CounterData ConsumeRate { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

### **TotalMessages**

```csharp
public static CounterData TotalMessages { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

### **Duration**

```csharp
public static CounterData Duration { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

### **DurationBase**

```csharp
public static CounterData DurationBase { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

### **TotalFaults**

```csharp
public static CounterData TotalFaults { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

### **FaultPercentage**

```csharp
public static CounterData FaultPercentage { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

### **FaultPercentageBase**

```csharp
public static CounterData FaultPercentageBase { get; }
```

#### Property Value

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>

## Constructors

### **ConsumerPerformanceCounters()**

```csharp
public ConsumerPerformanceCounters()
```

## Methods

### **Install()**

```csharp
public static void Install()
```

### **GetCounterData()**

```csharp
protected IEnumerable<CounterData> GetCounterData()
```

#### Returns

[IEnumerable\<CounterData\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
