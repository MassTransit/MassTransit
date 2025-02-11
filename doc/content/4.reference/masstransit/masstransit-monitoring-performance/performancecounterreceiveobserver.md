---

title: PerformanceCounterReceiveObserver

---

# PerformanceCounterReceiveObserver

Namespace: MassTransit.Monitoring.Performance

An observer that updates the performance counters using the bus events
 generated.

```csharp
public class PerformanceCounterReceiveObserver : IReceiveObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PerformanceCounterReceiveObserver](../masstransit-monitoring-performance/performancecounterreceiveobserver)<br/>
Implements [IReceiveObserver](../../masstransit-abstractions/masstransit/ireceiveobserver)

## Constructors

### **PerformanceCounterReceiveObserver(ICounterFactory)**

```csharp
public PerformanceCounterReceiveObserver(ICounterFactory factory)
```

#### Parameters

`factory` [ICounterFactory](../masstransit-monitoring-performance/icounterfactory)<br/>
