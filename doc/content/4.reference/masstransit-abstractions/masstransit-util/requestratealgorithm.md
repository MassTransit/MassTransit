---

title: RequestRateAlgorithm

---

# RequestRateAlgorithm

Namespace: MassTransit.Util

```csharp
public class RequestRateAlgorithm : IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestRateAlgorithm](../masstransit-util/requestratealgorithm)<br/>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **RequestCount**

The number of concurrent requests that should be performed based upon current response volume

```csharp
public int RequestCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ResultLimit**

The number of results that should be requested for each request

```csharp
public int ResultLimit { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ActiveRequestCount**

The current active request count

```csharp
public int ActiveRequestCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **MaxActiveRequestCount**

The maximum number of active requests that were made concurrently

```csharp
public int MaxActiveRequestCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **RequestRateAlgorithm(RequestRateAlgorithmOptions)**

```csharp
public RequestRateAlgorithm(RequestRateAlgorithmOptions options)
```

#### Parameters

`options` [RequestRateAlgorithmOptions](../masstransit-util/requestratealgorithmoptions)<br/>

## Methods

### **Dispose()**

```csharp
public void Dispose()
```

### **Run(RequestCallback, CancellationToken)**

Run a series of requests, up the limits, as a single pass

```csharp
public Task<int> Run(RequestCallback requestCallback, CancellationToken cancellationToken)
```

#### Parameters

`requestCallback` [RequestCallback](../masstransit-util/requestcallback)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Run\<T\>(RequestCallback\<T\>, ResultCallback\<T\>, CancellationToken)**

Run a series of requests, up the limits, as a single pass

```csharp
public Task<int> Run<T>(RequestCallback<T> requestCallback, ResultCallback<T> resultCallback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`requestCallback` [RequestCallback\<T\>](../masstransit-util/requestcallback-1)<br/>

`resultCallback` [ResultCallback\<T\>](../masstransit-util/resultcallback-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Run\<T, TKey\>(RequestCallback\<T\>, ResultCallback\<T\>, GroupCallback\<T, TKey\>, OrderCallback\<T\>, CancellationToken)**

Run a series of requests, up the limits, as a single pass

```csharp
public Task<int> Run<T, TKey>(RequestCallback<T> requestCallback, ResultCallback<T> resultCallback, GroupCallback<T, TKey> groupCallback, OrderCallback<T> orderCallback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

`TKey`<br/>

#### Parameters

`requestCallback` [RequestCallback\<T\>](../masstransit-util/requestcallback-1)<br/>

`resultCallback` [ResultCallback\<T\>](../masstransit-util/resultcallback-1)<br/>

`groupCallback` [GroupCallback\<T, TKey\>](../masstransit-util/groupcallback-2)<br/>

`orderCallback` [OrderCallback\<T\>](../masstransit-util/ordercallback-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **BeginRequest(CancellationToken)**

```csharp
public Task<ActiveRequest> BeginRequest(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ActiveRequest\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **EndRequest(Int32, Int32, CancellationToken)**

```csharp
internal Task EndRequest(int count, int resultLimit, CancellationToken cancellationToken)
```

#### Parameters

`count` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`resultLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CancelRequest(Int32)**

```csharp
internal void CancelRequest(int resultLimit)
```

#### Parameters

`resultLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ChangeRateLimit(Int32, CancellationToken)**

```csharp
public Task ChangeRateLimit(int newRateLimit, CancellationToken cancellationToken)
```

#### Parameters

`newRateLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
