---

title: ScopedConsumeFilter<T, TFilter>

---

# ScopedConsumeFilter\<T, TFilter\>

Namespace: MassTransit.Middleware

```csharp
public class ScopedConsumeFilter<T, TFilter> : IFilter<ConsumeContext<T>>, IProbeSite
```

#### Type Parameters

`T`<br/>

`TFilter`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopedConsumeFilter\<T, TFilter\>](../masstransit-middleware/scopedconsumefilter-2)<br/>
Implements [IFilter\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ScopedConsumeFilter(IConsumeScopeProvider)**

```csharp
public ScopedConsumeFilter(IConsumeScopeProvider scopeProvider)
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
