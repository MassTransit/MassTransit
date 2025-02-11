---

title: ICounterFactory

---

# ICounterFactory

Namespace: MassTransit.Monitoring.Performance

```csharp
public interface ICounterFactory
```

## Methods

### **Create(CounterCategory, String, String)**

```csharp
IPerformanceCounter Create(CounterCategory category, string counterName, string instanceName)
```

#### Parameters

`category` [CounterCategory](../masstransit-monitoring-performance/countercategory)<br/>

`counterName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`instanceName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IPerformanceCounter](../masstransit-monitoring-performance/iperformancecounter)<br/>
