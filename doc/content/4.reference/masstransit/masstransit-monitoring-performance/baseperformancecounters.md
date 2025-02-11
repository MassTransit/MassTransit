---

title: BasePerformanceCounters

---

# BasePerformanceCounters

Namespace: MassTransit.Monitoring.Performance

```csharp
public abstract class BasePerformanceCounters
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BasePerformanceCounters](../masstransit-monitoring-performance/baseperformancecounters)

## Methods

### **GetCounterData()**

```csharp
protected abstract IEnumerable<CounterData> GetCounterData()
```

#### Returns

[IEnumerable\<CounterData\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Convert(Counter, CounterType)**

```csharp
protected CounterData Convert(Counter counter, CounterType type)
```

#### Parameters

`counter` [Counter](../masstransit-monitoring-performance/counter)<br/>

`type` [CounterType](../masstransit-monitoring-performance/countertype)<br/>

#### Returns

[CounterData](../masstransit-monitoring-performance/counterdata)<br/>
