---

title: RequestRateAlgorithmOptions

---

# RequestRateAlgorithmOptions

Namespace: MassTransit.Util

```csharp
public class RequestRateAlgorithmOptions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestRateAlgorithmOptions](../masstransit-util/requestratealgorithmoptions)

## Properties

### **PrefetchCount**

The number of messages to keep in the pipeline at any given time

```csharp
public int PrefetchCount { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ConcurrentResultLimit**

The number of messages to dispatch concurrently

```csharp
public Nullable<int> ConcurrentResultLimit { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **RequestResultLimit**

The maximum number of results that can be retrieved per request

```csharp
public int RequestResultLimit { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **RequestRateLimit**

The maximum number of requests within the given request rate interval

```csharp
public Nullable<int> RequestRateLimit { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **RequestRateInterval**

The interval at which the request rate limit is reset

```csharp
public Nullable<TimeSpan> RequestRateInterval { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **RequestCancellationTimeout**

If specified, provides additional time when a request is canceled to avoid interrupting in-progress requests

```csharp
public Nullable<TimeSpan> RequestCancellationTimeout { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **RequestRateAlgorithmOptions()**

```csharp
public RequestRateAlgorithmOptions()
```
