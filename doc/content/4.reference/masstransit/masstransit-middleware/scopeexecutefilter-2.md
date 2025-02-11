---

title: ScopeExecuteFilter<TActivity, TArguments>

---

# ScopeExecuteFilter\<TActivity, TArguments\>

Namespace: MassTransit.Middleware

```csharp
public class ScopeExecuteFilter<TActivity, TArguments> : IFilter<ExecuteContext<TArguments>>, IProbeSite
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopeExecuteFilter\<TActivity, TArguments\>](../masstransit-middleware/scopeexecutefilter-2)<br/>
Implements [IFilter\<ExecuteContext\<TArguments\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ScopeExecuteFilter(IExecuteActivityScopeProvider\<TActivity, TArguments\>)**

```csharp
public ScopeExecuteFilter(IExecuteActivityScopeProvider<TActivity, TArguments> scopeProvider)
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
