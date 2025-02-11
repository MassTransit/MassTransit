---

title: CompensateActivityFactoryFilter<TActivity, TLog>

---

# CompensateActivityFactoryFilter\<TActivity, TLog\>

Namespace: MassTransit.Middleware

```csharp
public class CompensateActivityFactoryFilter<TActivity, TLog> : IFilter<CompensateContext<TLog>>, IProbeSite
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CompensateActivityFactoryFilter\<TActivity, TLog\>](../masstransit-middleware/compensateactivityfactoryfilter-2)<br/>
Implements [IFilter\<CompensateContext\<TLog\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **CompensateActivityFactoryFilter(ICompensateActivityFactory\<TActivity, TLog\>, IPipe\<CompensateActivityContext\<TActivity, TLog\>\>)**

```csharp
public CompensateActivityFactoryFilter(ICompensateActivityFactory<TActivity, TLog> factory, IPipe<CompensateActivityContext<TActivity, TLog>> pipe)
```

#### Parameters

`factory` [ICompensateActivityFactory\<TActivity, TLog\>](../../masstransit-abstractions/masstransit/icompensateactivityfactory-2)<br/>

`pipe` [IPipe\<CompensateActivityContext\<TActivity, TLog\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

## Methods

### **Send(CompensateContext\<TLog\>, IPipe\<CompensateContext\<TLog\>\>)**

```csharp
public Task Send(CompensateContext<TLog> context, IPipe<CompensateContext<TLog>> next)
```

#### Parameters

`context` [CompensateContext\<TLog\>](../../masstransit-abstractions/masstransit/compensatecontext-1)<br/>

`next` [IPipe\<CompensateContext\<TLog\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
