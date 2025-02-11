---

title: ConsumerPerformanceCounter

---

# ConsumerPerformanceCounter

Namespace: MassTransit.Monitoring.Performance

Tracks the consumption and failure of a consumer processing messages. The message types
 in this case are not included in the counter, only the consumer itself.

```csharp
public class ConsumerPerformanceCounter : IDisposable, IConsumerPerformanceCounter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerPerformanceCounter](../masstransit-monitoring-performance/consumerperformancecounter)<br/>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable), [IConsumerPerformanceCounter](../masstransit-monitoring-performance/iconsumerperformancecounter)

## Constructors

### **ConsumerPerformanceCounter(ICounterFactory, String)**

```csharp
public ConsumerPerformanceCounter(ICounterFactory factory, string consumerType)
```

#### Parameters

`factory` [ICounterFactory](../masstransit-monitoring-performance/icounterfactory)<br/>

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **Consumed(TimeSpan)**

```csharp
public void Consumed(TimeSpan duration)
```

#### Parameters

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **Faulted()**

```csharp
public void Faulted()
```

### **Dispose()**

```csharp
public void Dispose()
```
