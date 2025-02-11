---

title: StatsDCounterFactory

---

# StatsDCounterFactory

Namespace: MassTransit.Monitoring.Performance.StatsD

```csharp
public class StatsDCounterFactory : ICounterFactory
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StatsDCounterFactory](../masstransit-monitoring-performance-statsd/statsdcounterfactory)<br/>
Implements [ICounterFactory](../masstransit-monitoring-performance/icounterfactory)

## Constructors

### **StatsDCounterFactory(StatsDConfiguration)**

```csharp
public StatsDCounterFactory(StatsDConfiguration config)
```

#### Parameters

`config` [StatsDConfiguration](../masstransit-monitoring-performance-statsd/statsdconfiguration)<br/>

## Methods

### **Create(CounterCategory, String, String)**

```csharp
public IPerformanceCounter Create(CounterCategory category, string counterName, string instanceName)
```

#### Parameters

`category` [CounterCategory](../masstransit-monitoring-performance/countercategory)<br/>

`counterName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`instanceName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IPerformanceCounter](../masstransit-monitoring-performance/iperformancecounter)<br/>
