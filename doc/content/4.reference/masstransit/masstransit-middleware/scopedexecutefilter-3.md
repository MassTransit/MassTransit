---

title: ScopedExecuteFilter<TActivity, TArguments, TFilter>

---

# ScopedExecuteFilter\<TActivity, TArguments, TFilter\>

Namespace: MassTransit.Middleware

```csharp
public class ScopedExecuteFilter<TActivity, TArguments, TFilter> : IFilter<ExecuteContext<TArguments>>, IProbeSite
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

`TFilter`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopedExecuteFilter\<TActivity, TArguments, TFilter\>](../masstransit-middleware/scopedexecutefilter-3)<br/>
Implements [IFilter\<ExecuteContext\<TArguments\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ScopedExecuteFilter(IExecuteActivityScopeProvider\<TActivity, TArguments\>)**

```csharp
public ScopedExecuteFilter(IExecuteActivityScopeProvider<TActivity, TArguments> scopeProvider)
```

#### Parameters

`scopeProvider` [IExecuteActivityScopeProvider\<TActivity, TArguments\>](../masstransit-dependencyinjection/iexecuteactivityscopeprovider-2)<br/>

## Methods

### **Send(ExecuteContext\<TArguments\>, IPipe\<ExecuteContext\<TArguments\>\>)**

```csharp
public Task Send(ExecuteContext<TArguments> context, IPipe<ExecuteContext<TArguments>> next)
```

#### Parameters

`context` [ExecuteContext\<TArguments\>](../../masstransit-abstractions/masstransit/executecontext-1)<br/>

`next` [IPipe\<ExecuteContext\<TArguments\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
