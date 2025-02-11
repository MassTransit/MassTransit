---

title: IntervalRetryPolicy

---

# IntervalRetryPolicy

Namespace: MassTransit.RetryPolicies

```csharp
public class IntervalRetryPolicy : IRetryPolicy, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [IntervalRetryPolicy](../masstransit-retrypolicies/intervalretrypolicy)<br/>
Implements [IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **Intervals**

```csharp
public TimeSpan[] Intervals { get; }
```

#### Property Value

[TimeSpan[]](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Constructors

### **IntervalRetryPolicy(IExceptionFilter, TimeSpan[])**

```csharp
public IntervalRetryPolicy(IExceptionFilter filter, TimeSpan[] intervals)
```

#### Parameters

`filter` [IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

`intervals` [TimeSpan[]](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **IntervalRetryPolicy(IExceptionFilter, Int32[])**

```csharp
public IntervalRetryPolicy(IExceptionFilter filter, Int32[] intervals)
```

#### Parameters

`filter` [IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

`intervals` [Int32[]](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **IsHandled(Exception)**

```csharp
public bool IsHandled(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
