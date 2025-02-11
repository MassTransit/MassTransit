---

title: BuildRoutingSlipExecutor<TInput>

---

# BuildRoutingSlipExecutor\<TInput\>

Namespace: MassTransit.Futures

```csharp
public class BuildRoutingSlipExecutor<TInput> : IRoutingSlipExecutor<TInput>
```

#### Type Parameters

`TInput`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BuildRoutingSlipExecutor\<TInput\>](../masstransit-futures/buildroutingslipexecutor-1)<br/>
Implements [IRoutingSlipExecutor\<TInput\>](../masstransit-futures/iroutingslipexecutor-1)

## Properties

### **TrackRoutingSlip**

```csharp
public bool TrackRoutingSlip { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **BuildRoutingSlipExecutor(BuildItineraryCallback\<TInput\>)**

```csharp
public BuildRoutingSlipExecutor(BuildItineraryCallback<TInput> buildItinerary)
```

#### Parameters

`buildItinerary` [BuildItineraryCallback\<TInput\>](../masstransit/builditinerarycallback-1)<br/>

## Methods

### **Execute(BehaviorContext\<FutureState, TInput\>)**

```csharp
public Task Execute(BehaviorContext<FutureState, TInput> context)
```

#### Parameters

`context` [BehaviorContext\<FutureState, TInput\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
