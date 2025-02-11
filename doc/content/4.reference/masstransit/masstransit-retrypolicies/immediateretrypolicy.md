---

title: ImmediateRetryPolicy

---

# ImmediateRetryPolicy

Namespace: MassTransit.RetryPolicies

```csharp
public class ImmediateRetryPolicy : IRetryPolicy, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ImmediateRetryPolicy](../masstransit-retrypolicies/immediateretrypolicy)<br/>
Implements [IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **RetryLimit**

```csharp
public int RetryLimit { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **ImmediateRetryPolicy(IExceptionFilter, Int32)**

```csharp
public ImmediateRetryPolicy(IExceptionFilter filter, int retryLimit)
```

#### Parameters

`filter` [IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

`retryLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **IsHandled(Exception)**

```csharp
public bool IsHandled(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
