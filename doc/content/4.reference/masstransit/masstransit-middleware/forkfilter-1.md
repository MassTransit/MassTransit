---

title: ForkFilter<TContext>

---

# ForkFilter\<TContext\>

Namespace: MassTransit.Middleware

Forks a single pipe into two pipes, which are executed concurrently

```csharp
public class ForkFilter<TContext> : IFilter<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ForkFilter\<TContext\>](../masstransit-middleware/forkfilter-1)<br/>
Implements [IFilter\<TContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ForkFilter(IPipe\<TContext\>)**

```csharp
public ForkFilter(IPipe<TContext> pipe)
```

#### Parameters

`pipe` [IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>
