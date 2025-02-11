---

title: PlanRoutingSlipExecutor<TInput>

---

# PlanRoutingSlipExecutor\<TInput\>

Namespace: MassTransit.Futures

```csharp
public class PlanRoutingSlipExecutor<TInput> : IRoutingSlipExecutor<TInput>
```

#### Type Parameters

`TInput`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PlanRoutingSlipExecutor\<TInput\>](../masstransit-futures/planroutingslipexecutor-1)<br/>
Implements [IRoutingSlipExecutor\<TInput\>](../masstransit-futures/iroutingslipexecutor-1)

## Properties

### **TrackRoutingSlip**

```csharp
public bool TrackRoutingSlip { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **PlanRoutingSlipExecutor()**

```csharp
public PlanRoutingSlipExecutor()
```

## Methods

### **Execute(BehaviorContext\<FutureState, TInput\>)**

```csharp
public Task Execute(BehaviorContext<FutureState, TInput> context)
```

#### Parameters

`context` [BehaviorContext\<FutureState, TInput\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
