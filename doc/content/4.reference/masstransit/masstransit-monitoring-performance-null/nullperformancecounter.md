---

title: NullPerformanceCounter

---

# NullPerformanceCounter

Namespace: MassTransit.Monitoring.Performance.Null

```csharp
public class NullPerformanceCounter : IPerformanceCounter, IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [NullPerformanceCounter](../masstransit-monitoring-performance-null/nullperformancecounter)<br/>
Implements [IPerformanceCounter](../masstransit-monitoring-performance/iperformancecounter), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Constructors

### **NullPerformanceCounter()**

```csharp
public NullPerformanceCounter()
```

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
