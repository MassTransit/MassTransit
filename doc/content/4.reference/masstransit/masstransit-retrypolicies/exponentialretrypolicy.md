---

title: ExponentialRetryPolicy

---

# ExponentialRetryPolicy

Namespace: MassTransit.RetryPolicies

```csharp
public class ExponentialRetryPolicy : IRetryPolicy, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExponentialRetryPolicy](../masstransit-retrypolicies/exponentialretrypolicy)<br/>
Implements [IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **RetryLimit**

```csharp
public int RetryLimit { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **ExponentialRetryPolicy(IExceptionFilter, Int32, TimeSpan, TimeSpan, TimeSpan)**

```csharp
public ExponentialRetryPolicy(IExceptionFilter filter, int retryLimit, TimeSpan minInterval, TimeSpan maxInterval, TimeSpan intervalDelta)
```

#### Parameters

`filter` [IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

`retryLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`minInterval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`maxInterval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`intervalDelta` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Methods

### **IsHandled(Exception)**

```csharp
public bool IsHandled(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetRetryInterval(Int32)**

```csharp
public TimeSpan GetRetryInterval(int retryCount)
```

#### Parameters

`retryCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
