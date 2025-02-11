---

title: InterceptFilter<TContext>

---

# InterceptFilter\<TContext\>

Namespace: MassTransit.Middleware

Intercepts the pipe and executes an adjacent pipe prior to executing the next filter in the main pipe

```csharp
public class InterceptFilter<TContext> : IFilter<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InterceptFilter\<TContext\>](../masstransit-middleware/interceptfilter-1)<br/>
Implements [IFilter\<TContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **InterceptFilter(IPipe\<TContext\>)**

```csharp
public InterceptFilter(IPipe<TContext> pipe)
```

#### Parameters

`pipe` [IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>
