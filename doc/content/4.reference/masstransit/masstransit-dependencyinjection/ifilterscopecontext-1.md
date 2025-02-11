---

title: IFilterScopeContext<TContext>

---

# IFilterScopeContext\<TContext\>

Namespace: MassTransit.DependencyInjection

```csharp
public interface IFilterScopeContext<TContext> : IAsyncDisposable
```

#### Type Parameters

`TContext`<br/>

Implements [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **Filter**

```csharp
public abstract IFilter<TContext> Filter { get; }
```

#### Property Value

[IFilter\<TContext\>](../../masstransit-abstractions/masstransit/ifilter-1)<br/>

### **Context**

```csharp
public abstract TContext Context { get; }
```

#### Property Value

TContext<br/>
