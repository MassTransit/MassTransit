---

title: FaultedBehavior<TSaga, TMessage>

---

# FaultedBehavior\<TSaga, TMessage\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class FaultedBehavior<TSaga, TMessage> : IBehavior<TSaga, TMessage>, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FaultedBehavior\<TSaga, TMessage\>](../masstransit-sagastatemachine/faultedbehavior-2)<br/>
Implements [IBehavior\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/ibehavior-2), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **FaultedBehavior()**

```csharp
public FaultedBehavior()
```

## Methods

### **Accept(StateMachineVisitor)**

```csharp
public void Accept(StateMachineVisitor visitor)
```

#### Parameters

`visitor` [StateMachineVisitor](../../masstransit-abstractions/masstransit/statemachinevisitor)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Execute(BehaviorContext\<TSaga, TMessage\>)**

```csharp
public Task Execute(BehaviorContext<TSaga, TMessage> context)
```

#### Parameters

`context` [BehaviorContext\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted\<TException\>(BehaviorExceptionContext\<TSaga, TMessage, TException\>)**

```csharp
public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context)
```

#### Type Parameters

`TException`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, TMessage, TException\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-3)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
