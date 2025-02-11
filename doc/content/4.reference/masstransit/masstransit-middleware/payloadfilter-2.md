---

title: PayloadFilter<TContext, TPayload>

---

# PayloadFilter\<TContext, TPayload\>

Namespace: MassTransit.Middleware

```csharp
public class PayloadFilter<TContext, TPayload> : IFilter<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>

`TPayload`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PayloadFilter\<TContext, TPayload\>](../masstransit-middleware/payloadfilter-2)<br/>
Implements [IFilter\<TContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **PayloadFilter(TPayload)**

```csharp
public PayloadFilter(TPayload payload)
```

#### Parameters

`payload` TPayload<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Send(TContext, IPipe\<TContext\>)**

```csharp
public Task Send(TContext context, IPipe<TContext> next)
```

#### Parameters

`context` TContext<br/>

`next` [IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
