---

title: ActiveRequest

---

# ActiveRequest

Namespace: MassTransit.Util

```csharp
public struct ActiveRequest
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [ActiveRequest](../masstransit-util/activerequest)<br/>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Fields

### **CancellationToken**

```csharp
public CancellationToken CancellationToken;
```

### **ResultLimit**

```csharp
public int ResultLimit;
```

## Constructors

### **ActiveRequest(RequestRateAlgorithm, Int32, CancellationToken, TimeSpan)**

```csharp
public ActiveRequest(RequestRateAlgorithm algorithm, int resultLimit, CancellationToken cancellationToken, TimeSpan timeout)
```

#### Parameters

`algorithm` [RequestRateAlgorithm](../masstransit-util/requestratealgorithm)<br/>

`resultLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Methods

### **Complete(Int32, CancellationToken)**

```csharp
public Task Complete(int count, CancellationToken cancellationToken)
```

#### Parameters

`count` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Dispose()**

```csharp
public void Dispose()
```
