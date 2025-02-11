---

title: IItineraryPlanner<TInput>

---

# IItineraryPlanner\<TInput\>

Namespace: MassTransit

Implement to build a routing slip. This can be resolved by a durable future to build
 a routing slip at runtime in response to an input command.

```csharp
public interface IItineraryPlanner<TInput>
```

#### Type Parameters

`TInput`<br/>
The input message type

## Methods

### **PlanItinerary(BehaviorContext\<FutureState, TInput\>, IItineraryBuilder)**

```csharp
Task PlanItinerary(BehaviorContext<FutureState, TInput> value, IItineraryBuilder builder)
```

#### Parameters

`value` [BehaviorContext\<FutureState, TInput\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`builder` [IItineraryBuilder](../../masstransit-abstractions/masstransit/iitinerarybuilder)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
