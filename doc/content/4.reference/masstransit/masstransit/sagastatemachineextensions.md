---

title: SagaStateMachineExtensions

---

# SagaStateMachineExtensions

Namespace: MassTransit

```csharp
public static class SagaStateMachineExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaStateMachineExtensions](../masstransit/sagastatemachineextensions)

## Methods

### **CreateSagaQuery\<TInstance\>(StateMachine\<TInstance\>, Expression\<Func\<TInstance, Boolean\>\>, State[])**

Create a query that combines the specified expression with an expression that compares the instance state with the specified states

```csharp
public static ISagaQuery<TInstance> CreateSagaQuery<TInstance>(StateMachine<TInstance> machine, Expression<Func<TInstance, bool>> expression, State[] states)
```

#### Type Parameters

`TInstance`<br/>
The instance type

#### Parameters

`machine` [StateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/statemachine-1)<br/>
The state machine

`expression` Expression\<Func\<TInstance, Boolean\>\><br/>
The query expression

`states` [State[]](../../masstransit-abstractions/masstransit/state)<br/>
The states that are valid for this query

#### Returns

[ISagaQuery\<TInstance\>](../../masstransit-abstractions/masstransit/isagaquery-1)<br/>

### **CreateSagaFilter\<TInstance\>(StateMachine\<TInstance\>, Expression\<Func\<TInstance, Boolean\>\>, State[])**

Create a query that combines the specified expression with an expression that compares the instance state with the specified states

```csharp
public static Func<TInstance, bool> CreateSagaFilter<TInstance>(StateMachine<TInstance> machine, Expression<Func<TInstance, bool>> expression, State[] states)
```

#### Type Parameters

`TInstance`<br/>
The instance type

#### Parameters

`machine` [StateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/statemachine-1)<br/>
The state machine

`expression` Expression\<Func\<TInstance, Boolean\>\><br/>
The query expression

`states` [State[]](../../masstransit-abstractions/masstransit/state)<br/>
The states that are valid for this query

#### Returns

[Func\<TInstance, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
