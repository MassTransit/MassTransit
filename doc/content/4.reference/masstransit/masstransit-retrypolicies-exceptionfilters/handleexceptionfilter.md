---

title: HandleExceptionFilter

---

# HandleExceptionFilter

Namespace: MassTransit.RetryPolicies.ExceptionFilters

```csharp
public class HandleExceptionFilter : IExceptionFilter, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [HandleExceptionFilter](../masstransit-retrypolicies-exceptionfilters/handleexceptionfilter)<br/>
Implements [IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **HandleExceptionFilter(Type[])**

```csharp
public HandleExceptionFilter(Type[] exceptionTypes)
```

#### Parameters

`exceptionTypes` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
