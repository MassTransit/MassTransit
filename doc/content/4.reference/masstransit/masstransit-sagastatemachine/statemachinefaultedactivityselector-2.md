---

title: StateMachineFaultedActivitySelector<TSaga, TException>

---

# StateMachineFaultedActivitySelector\<TSaga, TException\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class StateMachineFaultedActivitySelector<TSaga, TException> : IStateMachineFaultedActivitySelector<TSaga, TException>
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StateMachineFaultedActivitySelector\<TSaga, TException\>](../masstransit-sagastatemachine/statemachinefaultedactivityselector-2)<br/>
Implements [IStateMachineFaultedActivitySelector\<TSaga, TException\>](../masstransit/istatemachinefaultedactivityselector-2)

## Constructors

### **StateMachineFaultedActivitySelector(ExceptionActivityBinder\<TSaga, TException\>)**

```csharp
public StateMachineFaultedActivitySelector(ExceptionActivityBinder<TSaga, TException> binder)
```

#### Parameters

`binder` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

## Methods

### **OfType\<TActivity\>()**

```csharp
public ExceptionActivityBinder<TSaga, TException> OfType<TActivity>()
```

#### Type Parameters

`TActivity`<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>
