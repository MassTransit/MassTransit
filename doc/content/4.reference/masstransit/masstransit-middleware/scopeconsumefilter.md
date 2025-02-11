---

title: ScopeConsumeFilter

---

# ScopeConsumeFilter

Namespace: MassTransit.Middleware

```csharp
public class ScopeConsumeFilter : IFilter<ConsumeContext>, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopeConsumeFilter](../masstransit-middleware/scopeconsumefilter)<br/>
Implements [IFilter\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ScopeConsumeFilter(IConsumeScopeProvider)**

```csharp
public ScopeConsumeFilter(IConsumeScopeProvider scopeProvider)
```

#### Parameters

`scopeProvider` [IConsumeScopeProvider](../masstransit-dependencyinjection/iconsumescopeprovider)<br/>

## Methods

### **Send(ConsumeContext, IPipe\<ConsumeContext\>)**

```csharp
public Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
```

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`next` [IPipe\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
