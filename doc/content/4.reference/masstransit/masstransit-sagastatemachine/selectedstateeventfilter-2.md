---

title: SelectedStateEventFilter<TSaga, TMessage>

---

# SelectedStateEventFilter\<TSaga, TMessage\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class SelectedStateEventFilter<TSaga, TMessage> : IStateEventFilter<TSaga>
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SelectedStateEventFilter\<TSaga, TMessage\>](../masstransit-sagastatemachine/selectedstateeventfilter-2)<br/>
Implements [IStateEventFilter\<TSaga\>](../masstransit-sagastatemachine/istateeventfilter-1)

## Constructors

### **SelectedStateEventFilter(StateMachineCondition\<TSaga, TMessage\>)**

```csharp
public SelectedStateEventFilter(StateMachineCondition<TSaga, TMessage> filter)
```

#### Parameters

`filter` [StateMachineCondition\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/statemachinecondition-2)<br/>

## Methods

### **Filter\<T\>(BehaviorContext\<TSaga, T\>)**

```csharp
public bool Filter<T>(BehaviorContext<TSaga, T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorContext\<TSaga, T\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Filter(BehaviorContext\<TSaga\>)**

```csharp
public bool Filter(BehaviorContext<TSaga> context)
```

#### Parameters

`context` [BehaviorContext\<TSaga\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
