---

title: PerformanceCounterSendObserver

---

# PerformanceCounterSendObserver

Namespace: MassTransit.Monitoring.Performance

An observer that updates the performance counters using the bus events
 generated.

```csharp
public class PerformanceCounterSendObserver : ISendObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PerformanceCounterSendObserver](../masstransit-monitoring-performance/performancecountersendobserver)<br/>
Implements [ISendObserver](../../masstransit-abstractions/masstransit/isendobserver)

## Constructors

### **PerformanceCounterSendObserver(ICounterFactory)**

```csharp
public PerformanceCounterSendObserver(ICounterFactory factory)
```

#### Parameters

`factory` [ICounterFactory](../masstransit-monitoring-performance/icounterfactory)<br/>
