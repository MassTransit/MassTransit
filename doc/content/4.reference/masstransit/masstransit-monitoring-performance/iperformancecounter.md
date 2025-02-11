---

title: IPerformanceCounter

---

# IPerformanceCounter

Namespace: MassTransit.Monitoring.Performance

```csharp
public interface IPerformanceCounter : IDisposable
```

Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Methods

### **Increment()**

```csharp
void Increment()
```

### **IncrementBy(Int64)**

```csharp
void IncrementBy(long val)
```

#### Parameters

`val` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **Set(Int64)**

```csharp
void Set(long val)
```

#### Parameters

`val` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>
