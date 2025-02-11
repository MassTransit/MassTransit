---

title: IStateMachineActivity<TSaga, TMessage>

---

# IStateMachineActivity\<TSaga, TMessage\>

Namespace: MassTransit

An activity is part of a behavior that is executed in order

```csharp
public interface IStateMachineActivity<TSaga, TMessage> : IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Implements [IStateMachineActivity](../masstransit/istatemachineactivity), [IVisitable](../masstransit/ivisitable), [IProbeSite](../masstransit/iprobesite)

## Methods

### **Execute(BehaviorContext\<TSaga, TMessage\>, IBehavior\<TSaga, TMessage\>)**

Execute the activity with the given behavior context

```csharp
Task Execute(BehaviorContext<TSaga, TMessage> context, IBehavior<TSaga, TMessage> next)
```

#### Parameters

`context` [BehaviorContext\<TSaga, TMessage\>](../masstransit/behaviorcontext-2)<br/>
The behavior context

`next` [IBehavior\<TSaga, TMessage\>](../masstransit/ibehavior-2)<br/>
The behavior that follows this activity

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
An awaitable task

### **Faulted\<TException\>(BehaviorExceptionContext\<TSaga, TMessage, TException\>, IBehavior\<TSaga, TMessage\>)**

The exception path through the behavior allows activities to catch and handle exceptions

```csharp
Task Faulted<TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context, IBehavior<TSaga, TMessage> next)
```

#### Type Parameters

`TException`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, TMessage, TException\>](../masstransit/behaviorexceptioncontext-3)<br/>

`next` [IBehavior\<TSaga, TMessage\>](../masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
