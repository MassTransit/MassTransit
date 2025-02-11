---

title: ExecuteActivityHost<TActivity, TArguments>

---

# ExecuteActivityHost\<TActivity, TArguments\>

Namespace: MassTransit.Courier

```csharp
public class ExecuteActivityHost<TActivity, TArguments> : IFilter<ConsumeContext<RoutingSlip>>, IProbeSite
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExecuteActivityHost\<TActivity, TArguments\>](../masstransit-courier/executeactivityhost-2)<br/>
Implements [IFilter\<ConsumeContext\<RoutingSlip\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ExecuteActivityHost(IPipe\<ExecuteContext\<TArguments\>\>, Uri)**

```csharp
public ExecuteActivityHost(IPipe<ExecuteContext<TArguments>> executePipe, Uri compensateAddress)
```

#### Parameters

`executePipe` [IPipe\<ExecuteContext\<TArguments\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`compensateAddress` Uri<br/>

## Methods

### **Send(ConsumeContext\<RoutingSlip\>, IPipe\<ConsumeContext\<RoutingSlip\>\>)**

```csharp
public Task Send(ConsumeContext<RoutingSlip> context, IPipe<ConsumeContext<RoutingSlip>> next)
```

#### Parameters

`context` [ConsumeContext\<RoutingSlip\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`next` [IPipe\<ConsumeContext\<RoutingSlip\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
