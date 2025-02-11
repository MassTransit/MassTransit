---

title: ScopeCompensateFilter<TActivity, TLog>

---

# ScopeCompensateFilter\<TActivity, TLog\>

Namespace: MassTransit.Middleware

```csharp
public class ScopeCompensateFilter<TActivity, TLog> : IFilter<CompensateContext<TLog>>, IProbeSite
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopeCompensateFilter\<TActivity, TLog\>](../masstransit-middleware/scopecompensatefilter-2)<br/>
Implements [IFilter\<CompensateContext\<TLog\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ScopeCompensateFilter(ICompensateActivityScopeProvider\<TActivity, TLog\>)**

```csharp
public ScopeCompensateFilter(ICompensateActivityScopeProvider<TActivity, TLog> scopeProvider)
```

#### Parameters

`scopeProvider` [ICompensateActivityScopeProvider\<TActivity, TLog\>](../masstransit-dependencyinjection/icompensateactivityscopeprovider-2)<br/>

## Methods

### **Send(CompensateContext\<TLog\>, IPipe\<CompensateContext\<TLog\>\>)**

```csharp
public Task Send(CompensateContext<TLog> context, IPipe<CompensateContext<TLog>> next)
```

#### Parameters

`context` [CompensateContext\<TLog\>](../../masstransit-abstractions/masstransit/compensatecontext-1)<br/>

`next` [IPipe\<CompensateContext\<TLog\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
