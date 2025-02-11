---

title: PerformanceCounterPublishObserver

---

# PerformanceCounterPublishObserver

Namespace: MassTransit.Monitoring.Performance

An observer that updates the performance counters using the bus events
 generated.

```csharp
public class PerformanceCounterPublishObserver : IPublishObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PerformanceCounterPublishObserver](../masstransit-monitoring-performance/performancecounterpublishobserver)<br/>
Implements [IPublishObserver](../../masstransit-abstractions/masstransit/ipublishobserver)

## Constructors

### **PerformanceCounterPublishObserver(ICounterFactory)**

```csharp
public PerformanceCounterPublishObserver(ICounterFactory factory)
```

#### Parameters

`factory` [ICounterFactory](../masstransit-monitoring-performance/icounterfactory)<br/>
