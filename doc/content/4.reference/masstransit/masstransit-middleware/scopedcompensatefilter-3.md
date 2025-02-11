---

title: ScopedCompensateFilter<TActivity, TArguments, TFilter>

---

# ScopedCompensateFilter\<TActivity, TArguments, TFilter\>

Namespace: MassTransit.Middleware

```csharp
public class ScopedCompensateFilter<TActivity, TArguments, TFilter> : IFilter<CompensateContext<TArguments>>, IProbeSite
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

`TFilter`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopedCompensateFilter\<TActivity, TArguments, TFilter\>](../masstransit-middleware/scopedcompensatefilter-3)<br/>
Implements [IFilter\<CompensateContext\<TArguments\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ScopedCompensateFilter(ICompensateActivityScopeProvider\<TActivity, TArguments\>)**

```csharp
public ScopedCompensateFilter(ICompensateActivityScopeProvider<TActivity, TArguments> scopeProvider)
```

#### Parameters

`scopeProvider` [ICompensateActivityScopeProvider\<TActivity, TArguments\>](../masstransit-dependencyinjection/icompensateactivityscopeprovider-2)<br/>

## Methods

### **Send(CompensateContext\<TArguments\>, IPipe\<CompensateContext\<TArguments\>\>)**

```csharp
public Task Send(CompensateContext<TArguments> context, IPipe<CompensateContext<TArguments>> next)
```

#### Parameters

`context` [CompensateContext\<TArguments\>](../../masstransit-abstractions/masstransit/compensatecontext-1)<br/>

`next` [IPipe\<CompensateContext\<TArguments\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
