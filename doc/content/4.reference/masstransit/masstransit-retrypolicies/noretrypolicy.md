---

title: NoRetryPolicy

---

# NoRetryPolicy

Namespace: MassTransit.RetryPolicies

```csharp
public class NoRetryPolicy : IRetryPolicy, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [NoRetryPolicy](../masstransit-retrypolicies/noretrypolicy)<br/>
Implements [IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **NoRetryPolicy(IExceptionFilter)**

```csharp
public NoRetryPolicy(IExceptionFilter filter)
```

#### Parameters

`filter` [IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

## Methods

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
