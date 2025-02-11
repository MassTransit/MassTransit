---

title: FilterPipe<TContext>

---

# FilterPipe\<TContext\>

Namespace: MassTransit.Middleware

```csharp
public class FilterPipe<TContext> : IPipe<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FilterPipe\<TContext\>](../masstransit-middleware/filterpipe-1)<br/>
Implements [IPipe\<TContext\>](../masstransit/ipipe-1), [IProbeSite](../masstransit/iprobesite)

## Constructors

### **FilterPipe(IFilter\<TContext\>, IPipe\<TContext\>)**

```csharp
public FilterPipe(IFilter<TContext> filter, IPipe<TContext> next)
```

#### Parameters

`filter` [IFilter\<TContext\>](../masstransit/ifilter-1)<br/>

`next` [IPipe\<TContext\>](../masstransit/ipipe-1)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../masstransit/probecontext)<br/>

### **Send(TContext)**

```csharp
public Task Send(TContext context)
```

#### Parameters

`context` TContext<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
