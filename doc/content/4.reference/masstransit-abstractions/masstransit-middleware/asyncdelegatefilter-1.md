---

title: AsyncDelegateFilter<TContext>

---

# AsyncDelegateFilter\<TContext\>

Namespace: MassTransit.Middleware

```csharp
public class AsyncDelegateFilter<TContext> : IFilter<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncDelegateFilter\<TContext\>](../masstransit-middleware/asyncdelegatefilter-1)<br/>
Implements [IFilter\<TContext\>](../masstransit/ifilter-1), [IProbeSite](../masstransit/iprobesite)

## Constructors

### **AsyncDelegateFilter(Func\<TContext, Task\>)**

```csharp
public AsyncDelegateFilter(Func<TContext, Task> callback)
```

#### Parameters

`callback` [Func\<TContext, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

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
