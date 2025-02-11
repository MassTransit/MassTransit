---

title: ReceiveEndpointDependencyFilter<TContext>

---

# ReceiveEndpointDependencyFilter\<TContext\>

Namespace: MassTransit.Middleware

```csharp
public class ReceiveEndpointDependencyFilter<TContext> : IFilter<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveEndpointDependencyFilter\<TContext\>](../masstransit-middleware/receiveendpointdependencyfilter-1)<br/>
Implements [IFilter\<TContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ReceiveEndpointDependencyFilter(ReceiveEndpointContext)**

```csharp
public ReceiveEndpointDependencyFilter(ReceiveEndpointContext context)
```

#### Parameters

`context` [ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>

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
