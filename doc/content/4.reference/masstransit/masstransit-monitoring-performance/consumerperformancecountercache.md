---

title: ConsumerPerformanceCounterCache

---

# ConsumerPerformanceCounterCache

Namespace: MassTransit.Monitoring.Performance

```csharp
public class ConsumerPerformanceCounterCache
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerPerformanceCounterCache](../masstransit-monitoring-performance/consumerperformancecountercache)

## Methods

### **GetCounter(ICounterFactory, String)**

```csharp
public static IConsumerPerformanceCounter GetCounter(ICounterFactory factory, string consumerType)
```

#### Parameters

`factory` [ICounterFactory](../masstransit-monitoring-performance/icounterfactory)<br/>

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IConsumerPerformanceCounter](../masstransit-monitoring-performance/iconsumerperformancecounter)<br/>
