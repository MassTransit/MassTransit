---

title: FilterExceptionFilter<T>

---

# FilterExceptionFilter\<T\>

Namespace: MassTransit.RetryPolicies.ExceptionFilters

```csharp
public class FilterExceptionFilter<T> : IExceptionFilter, IProbeSite
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FilterExceptionFilter\<T\>](../masstransit-retrypolicies-exceptionfilters/filterexceptionfilter-1)<br/>
Implements [IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **FilterExceptionFilter(Func\<T, Boolean\>)**

```csharp
public FilterExceptionFilter(Func<T, bool> filter)
```

#### Parameters

`filter` [Func\<T, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
