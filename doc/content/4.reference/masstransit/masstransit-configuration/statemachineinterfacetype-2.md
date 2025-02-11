---

title: StateMachineInterfaceType<TInstance, TData>

---

# StateMachineInterfaceType\<TInstance, TData\>

Namespace: MassTransit.Configuration

```csharp
public class StateMachineInterfaceType<TInstance, TData> : IStateMachineInterfaceType
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StateMachineInterfaceType\<TInstance, TData\>](../masstransit-configuration/statemachineinterfacetype-2)<br/>
Implements [IStateMachineInterfaceType](../masstransit-configuration/istatemachineinterfacetype)

## Constructors

### **StateMachineInterfaceType(SagaStateMachine\<TInstance\>, EventCorrelation\<TInstance, TData\>)**

```csharp
public StateMachineInterfaceType(SagaStateMachine<TInstance> machine, EventCorrelation<TInstance, TData> correlation)
```

#### Parameters

`machine` [SagaStateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/sagastatemachine-1)<br/>

`correlation` [EventCorrelation\<TInstance, TData\>](../../masstransit-abstractions/masstransit/eventcorrelation-2)<br/>
