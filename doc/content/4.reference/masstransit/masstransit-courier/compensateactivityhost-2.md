---

title: CompensateActivityHost<TActivity, TLog>

---

# CompensateActivityHost\<TActivity, TLog\>

Namespace: MassTransit.Courier

```csharp
public class CompensateActivityHost<TActivity, TLog> : IFilter<ConsumeContext<RoutingSlip>>, IProbeSite
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CompensateActivityHost\<TActivity, TLog\>](../masstransit-courier/compensateactivityhost-2)<br/>
Implements [IFilter\<ConsumeContext\<RoutingSlip\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **CompensateActivityHost(IPipe\<CompensateContext\<TLog\>\>)**

```csharp
public CompensateActivityHost(IPipe<CompensateContext<TLog>> compensatePipe)
```

#### Parameters

`compensatePipe` [IPipe\<CompensateContext\<TLog\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

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
