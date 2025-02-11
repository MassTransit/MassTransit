---

title: LastBehavior<TSaga>

---

# LastBehavior\<TSaga\>

Namespace: MassTransit.SagaStateMachine

The last behavior either completes the last activity in the behavior or
 throws the exception if a compensation is in progress

```csharp
public class LastBehavior<TSaga> : IBehavior<TSaga>, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [LastBehavior\<TSaga\>](../masstransit-sagastatemachine/lastbehavior-1)<br/>
Implements [IBehavior\<TSaga\>](../../masstransit-abstractions/masstransit/ibehavior-1), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **LastBehavior(IStateMachineActivity\<TSaga\>)**

```csharp
public LastBehavior(IStateMachineActivity<TSaga> activity)
```

#### Parameters

`activity` [IStateMachineActivity\<TSaga\>](../../masstransit-abstractions/masstransit/istatemachineactivity-1)<br/>

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

### **Execute(BehaviorContext\<TSaga\>)**

```csharp
public Task Execute(BehaviorContext<TSaga> context)
```

#### Parameters

`context` [BehaviorContext\<TSaga\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Execute\<T\>(BehaviorContext\<TSaga, T\>)**

```csharp
public Task Execute<T>(BehaviorContext<TSaga, T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorContext\<TSaga, T\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted\<T, TException\>(BehaviorExceptionContext\<TSaga, T, TException\>)**

```csharp
public Task Faulted<T, TException>(BehaviorExceptionContext<TSaga, T, TException> context)
```

#### Type Parameters

`T`<br/>

`TException`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, T, TException\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-3)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted\<TException\>(BehaviorExceptionContext\<TSaga, TException\>)**

```csharp
public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TException> context)
```

#### Type Parameters

`TException`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, TException\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
