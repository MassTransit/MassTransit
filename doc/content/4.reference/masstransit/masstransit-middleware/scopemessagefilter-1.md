---

title: ScopeMessageFilter<T>

---

# ScopeMessageFilter\<T\>

Namespace: MassTransit.Middleware

```csharp
public class ScopeMessageFilter<T> : IFilter<ConsumeContext<T>>, IProbeSite
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopeMessageFilter\<T\>](../masstransit-middleware/scopemessagefilter-1)<br/>
Implements [IFilter\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ScopeMessageFilter(IConsumeScopeProvider)**

```csharp
public ScopeMessageFilter(IConsumeScopeProvider scopeProvider)
```

#### Parameters

`scopeProvider` [IConsumeScopeProvider](../masstransit-dependencyinjection/iconsumescopeprovider)<br/>

## Methods

### **Send(ConsumeContext\<T\>, IPipe\<ConsumeContext\<T\>\>)**

```csharp
public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
```

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`next` [IPipe\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
