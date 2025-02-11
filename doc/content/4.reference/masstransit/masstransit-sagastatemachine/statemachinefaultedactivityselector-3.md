---

title: StateMachineFaultedActivitySelector<TSaga, TMessage, TException>

---

# StateMachineFaultedActivitySelector\<TSaga, TMessage, TException\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class StateMachineFaultedActivitySelector<TSaga, TMessage, TException> : IStateMachineFaultedActivitySelector<TSaga, TMessage, TException>
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StateMachineFaultedActivitySelector\<TSaga, TMessage, TException\>](../masstransit-sagastatemachine/statemachinefaultedactivityselector-3)<br/>
Implements [IStateMachineFaultedActivitySelector\<TSaga, TMessage, TException\>](../masstransit/istatemachinefaultedactivityselector-3)

## Constructors

### **StateMachineFaultedActivitySelector(ExceptionActivityBinder\<TSaga, TMessage, TException\>)**

```csharp
public StateMachineFaultedActivitySelector(ExceptionActivityBinder<TSaga, TMessage, TException> binder)
```

#### Parameters

`binder` [ExceptionActivityBinder\<TSaga, TMessage, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

## Methods

### **OfType\<TActivity\>()**

```csharp
public ExceptionActivityBinder<TSaga, TMessage, TException> OfType<TActivity>()
```

#### Type Parameters

`TActivity`<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TMessage, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **OfInstanceType\<TActivity\>()**

```csharp
public ExceptionActivityBinder<TSaga, TMessage, TException> OfInstanceType<TActivity>()
```

#### Type Parameters

`TActivity`<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TMessage, TException\>](../masstransit/exceptionactivitybinder-3)<br/>
