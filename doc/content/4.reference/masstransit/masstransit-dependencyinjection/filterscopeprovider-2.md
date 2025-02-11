---

title: FilterScopeProvider<TFilter, TContext>

---

# FilterScopeProvider\<TFilter, TContext\>

Namespace: MassTransit.DependencyInjection

Used by Send/Publish filters to send within either a scoped endpoint/request client context or within the consume context
 currently active

```csharp
public class FilterScopeProvider<TFilter, TContext> : IFilterScopeProvider<TContext>, IProbeSite
```

#### Type Parameters

`TFilter`<br/>

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FilterScopeProvider\<TFilter, TContext\>](../masstransit-dependencyinjection/filterscopeprovider-2)<br/>
Implements [IFilterScopeProvider\<TContext\>](../masstransit-dependencyinjection/ifilterscopeprovider-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **FilterScopeProvider(IServiceProvider)**

```csharp
public FilterScopeProvider(IServiceProvider serviceProvider)
```

#### Parameters

`serviceProvider` IServiceProvider<br/>

## Methods

### **Create(TContext)**

```csharp
public IFilterScopeContext<TContext> Create(TContext context)
```

#### Parameters

`context` TContext<br/>

#### Returns

[IFilterScopeContext\<TContext\>](../masstransit-dependencyinjection/ifilterscopecontext-1)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
