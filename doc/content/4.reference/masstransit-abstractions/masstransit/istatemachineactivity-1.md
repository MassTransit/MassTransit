---

title: IStateMachineActivity<TSaga>

---

# IStateMachineActivity\<TSaga\>

Namespace: MassTransit

An activity is part of a behavior that is executed in order

```csharp
public interface IStateMachineActivity<TSaga> : IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

Implements [IStateMachineActivity](../masstransit/istatemachineactivity), [IVisitable](../masstransit/ivisitable), [IProbeSite](../masstransit/iprobesite)

## Methods

### **Execute(BehaviorContext\<TSaga\>, IBehavior\<TSaga\>)**

Execute the activity with the given behavior context

```csharp
Task Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
```

#### Parameters

`context` [BehaviorContext\<TSaga\>](../masstransit/behaviorcontext-1)<br/>
The behavior context

`next` [IBehavior\<TSaga\>](../masstransit/ibehavior-1)<br/>
The behavior that follows this activity

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
An awaitable task

### **Execute\<T\>(BehaviorContext\<TSaga, T\>, IBehavior\<TSaga, T\>)**

Execute the activity with the given behavior context

```csharp
Task Execute<T>(BehaviorContext<TSaga, T> context, IBehavior<TSaga, T> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorContext\<TSaga, T\>](../masstransit/behaviorcontext-2)<br/>
The behavior context

`next` [IBehavior\<TSaga, T\>](../masstransit/ibehavior-2)<br/>
The behavior that follows this activity

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
An awaitable task

### **Faulted\<TException\>(BehaviorExceptionContext\<TSaga, TException\>, IBehavior\<TSaga\>)**

The exception path through the behavior allows activities to catch and handle exceptions

```csharp
Task Faulted<TException>(BehaviorExceptionContext<TSaga, TException> context, IBehavior<TSaga> next)
```

#### Type Parameters

`TException`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, TException\>](../masstransit/behaviorexceptioncontext-2)<br/>

`next` [IBehavior\<TSaga\>](../masstransit/ibehavior-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted\<T, TException\>(BehaviorExceptionContext\<TSaga, T, TException\>, IBehavior\<TSaga, T\>)**

The exception path through the behavior allows activities to catch and handle exceptions

```csharp
Task Faulted<T, TException>(BehaviorExceptionContext<TSaga, T, TException> context, IBehavior<TSaga, T> next)
```

#### Type Parameters

`T`<br/>

`TException`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, T, TException\>](../masstransit/behaviorexceptioncontext-3)<br/>

`next` [IBehavior\<TSaga, T\>](../masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
