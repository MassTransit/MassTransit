---

title: DelegateFilter<TContext>

---

# DelegateFilter\<TContext\>

Namespace: MassTransit.Middleware

```csharp
public class DelegateFilter<TContext> : IFilter<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DelegateFilter\<TContext\>](../masstransit-middleware/delegatefilter-1)<br/>
Implements [IFilter\<TContext\>](../masstransit/ifilter-1), [IProbeSite](../masstransit/iprobesite)

## Constructors

### **DelegateFilter(Action\<TContext\>)**

```csharp
public DelegateFilter(Action<TContext> callback)
```

#### Parameters

`callback` [Action\<TContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

## Methods

### **Send(TContext, IPipe\<TContext\>)**

```csharp
public Task Send(TContext context, IPipe<TContext> next)
```

#### Parameters

`context` TContext<br/>

`next` [IPipe\<TContext\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
