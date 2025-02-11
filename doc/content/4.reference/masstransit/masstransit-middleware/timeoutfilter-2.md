---

title: TimeoutFilter<TContext, TResult>

---

# TimeoutFilter\<TContext, TResult\>

Namespace: MassTransit.Middleware

```csharp
public class TimeoutFilter<TContext, TResult> : IFilter<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>

`TResult`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TimeoutFilter\<TContext, TResult\>](../masstransit-middleware/timeoutfilter-2)<br/>
Implements [IFilter\<TContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **TimeoutFilter(Func\<TContext, CancellationToken, TResult\>, TimeSpan)**

```csharp
public TimeoutFilter(Func<TContext, CancellationToken, TResult> contextFactory, TimeSpan timeout)
```

#### Parameters

`contextFactory` [Func\<TContext, CancellationToken, TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

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
