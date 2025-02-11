---

title: IFilterScopeProvider<TContext>

---

# IFilterScopeProvider\<TContext\>

Namespace: MassTransit.DependencyInjection

```csharp
public interface IFilterScopeProvider<TContext> : IProbeSite
```

#### Type Parameters

`TContext`<br/>

Implements [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **Create(TContext)**

```csharp
IFilterScopeContext<TContext> Create(TContext context)
```

#### Parameters

`context` TContext<br/>

#### Returns

[IFilterScopeContext\<TContext\>](../masstransit-dependencyinjection/ifilterscopecontext-1)<br/>
