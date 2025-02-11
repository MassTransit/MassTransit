---

title: LastPipe<TContext>

---

# LastPipe\<TContext\>

Namespace: MassTransit.Middleware

The last pipe in a pipeline is always an end pipe that does nothing and returns synchronously

```csharp
public class LastPipe<TContext> : IPipe<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [LastPipe\<TContext\>](../masstransit-middleware/lastpipe-1)<br/>
Implements [IPipe\<TContext\>](../masstransit/ipipe-1), [IProbeSite](../masstransit/iprobesite)

## Constructors

### **LastPipe(IFilter\<TContext\>)**

```csharp
public LastPipe(IFilter<TContext> filter)
```

#### Parameters

`filter` [IFilter\<TContext\>](../masstransit/ifilter-1)<br/>

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
