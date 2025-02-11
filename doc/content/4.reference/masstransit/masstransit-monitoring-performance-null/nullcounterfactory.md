---

title: NullCounterFactory

---

# NullCounterFactory

Namespace: MassTransit.Monitoring.Performance.Null

```csharp
public class NullCounterFactory : ICounterFactory
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [NullCounterFactory](../masstransit-monitoring-performance-null/nullcounterfactory)<br/>
Implements [ICounterFactory](../masstransit-monitoring-performance/icounterfactory)

## Constructors

### **NullCounterFactory()**

```csharp
public NullCounterFactory()
```

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
