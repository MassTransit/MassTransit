---

title: IgnoreExceptionFilter

---

# IgnoreExceptionFilter

Namespace: MassTransit.RetryPolicies.ExceptionFilters

```csharp
public class IgnoreExceptionFilter : IExceptionFilter, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [IgnoreExceptionFilter](../masstransit-retrypolicies-exceptionfilters/ignoreexceptionfilter)<br/>
Implements [IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **IgnoreExceptionFilter(Type[])**

```csharp
public IgnoreExceptionFilter(Type[] exceptionTypes)
```

#### Parameters

`exceptionTypes` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
