---

title: IncrementalRetryPolicy

---

# IncrementalRetryPolicy

Namespace: MassTransit.RetryPolicies

```csharp
public class IncrementalRetryPolicy : IRetryPolicy, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [IncrementalRetryPolicy](../masstransit-retrypolicies/incrementalretrypolicy)<br/>
Implements [IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **RetryLimit**

```csharp
public int RetryLimit { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **InitialInterval**

```csharp
public TimeSpan InitialInterval { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **IntervalIncrement**

```csharp
public TimeSpan IntervalIncrement { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Constructors

### **IncrementalRetryPolicy(IExceptionFilter, Int32, TimeSpan, TimeSpan)**

```csharp
public IncrementalRetryPolicy(IExceptionFilter filter, int retryLimit, TimeSpan initialInterval, TimeSpan intervalIncrement)
```

#### Parameters

`filter` [IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

`retryLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`initialInterval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`intervalIncrement` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Methods

### **IsHandled(Exception)**

```csharp
public bool IsHandled(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
