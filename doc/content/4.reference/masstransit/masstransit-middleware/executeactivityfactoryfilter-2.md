---

title: ExecuteActivityFactoryFilter<TActivity, TArguments>

---

# ExecuteActivityFactoryFilter\<TActivity, TArguments\>

Namespace: MassTransit.Middleware

```csharp
public class ExecuteActivityFactoryFilter<TActivity, TArguments> : IFilter<ExecuteContext<TArguments>>, IProbeSite
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExecuteActivityFactoryFilter\<TActivity, TArguments\>](../masstransit-middleware/executeactivityfactoryfilter-2)<br/>
Implements [IFilter\<ExecuteContext\<TArguments\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ExecuteActivityFactoryFilter(IExecuteActivityFactory\<TActivity, TArguments\>, IPipe\<ExecuteActivityContext\<TActivity, TArguments\>\>)**

```csharp
public ExecuteActivityFactoryFilter(IExecuteActivityFactory<TActivity, TArguments> factory, IPipe<ExecuteActivityContext<TActivity, TArguments>> pipe)
```

#### Parameters

`factory` [IExecuteActivityFactory\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityfactory-2)<br/>

`pipe` [IPipe\<ExecuteActivityContext\<TActivity, TArguments\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

## Methods

### **Send(ExecuteContext\<TArguments\>, IPipe\<ExecuteContext\<TArguments\>\>)**

```csharp
public Task Send(ExecuteContext<TArguments> context, IPipe<ExecuteContext<TArguments>> next)
```

#### Parameters

`context` [ExecuteContext\<TArguments\>](../../masstransit-abstractions/masstransit/executecontext-1)<br/>

`next` [IPipe\<ExecuteContext\<TArguments\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
