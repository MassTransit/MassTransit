---

title: ContextFilter<TContext>

---

# ContextFilter\<TContext\>

Namespace: MassTransit.Middleware

A content filter applies a delegate to the message context, and uses the result to either accept the message
 or discard it.

```csharp
public class ContextFilter<TContext> : IFilter<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ContextFilter\<TContext\>](../masstransit-middleware/contextfilter-1)<br/>
Implements [IFilter\<TContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ContextFilter(Func\<TContext, Task\<Boolean\>\>)**

```csharp
public ContextFilter(Func<TContext, Task<bool>> filter)
```

#### Parameters

`filter` [Func\<TContext, Task\<Boolean\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

## Methods

### **Send(TContext, IPipe\<TContext\>)**

```csharp
public Task Send(TContext context, IPipe<TContext> next)
```

#### Parameters

`context` TContext<br/>

`next` [IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
