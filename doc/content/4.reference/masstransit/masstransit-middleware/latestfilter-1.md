---

title: LatestFilter<T>

---

# LatestFilter\<T\>

Namespace: MassTransit.Middleware

Retains the last value that was sent through the filter, usable as a source to a join pipe

```csharp
public class LatestFilter<T> : IFilter<T>, IProbeSite, ILatestFilter<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [LatestFilter\<T\>](../masstransit-middleware/latestfilter-1)<br/>
Implements [IFilter\<T\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [ILatestFilter\<T\>](../masstransit-middleware/ilatestfilter-1)

## Constructors

### **LatestFilter()**

```csharp
public LatestFilter()
```
