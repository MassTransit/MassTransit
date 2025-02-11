---

title: StatsDPerformanceCounter

---

# StatsDPerformanceCounter

Namespace: MassTransit.Monitoring.Performance.StatsD

```csharp
public class StatsDPerformanceCounter : IPerformanceCounter, IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StatsDPerformanceCounter](../masstransit-monitoring-performance-statsd/statsdperformancecounter)<br/>
Implements [IPerformanceCounter](../masstransit-monitoring-performance/iperformancecounter), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Constructors

### **StatsDPerformanceCounter(StatsDConfiguration, String, String, String)**

```csharp
public StatsDPerformanceCounter(StatsDConfiguration cfg, string category, string name, string instance)
```

#### Parameters

`cfg` [StatsDConfiguration](../masstransit-monitoring-performance-statsd/statsdconfiguration)<br/>

`category` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`instance` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **Increment()**

```csharp
public void Increment()
```

### **IncrementBy(Int64)**

```csharp
public void IncrementBy(long val)
```

#### Parameters

`val` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **Set(Int64)**

```csharp
public void Set(long val)
```

#### Parameters

`val` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **Dispose()**

```csharp
public void Dispose()
```
