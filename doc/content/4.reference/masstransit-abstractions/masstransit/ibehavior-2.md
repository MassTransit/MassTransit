---

title: IBehavior<TSaga, TMessage>

---

# IBehavior\<TSaga, TMessage\>

Namespace: MassTransit

A behavior is a chain of activities invoked by a state

```csharp
public interface IBehavior<TSaga, TMessage> : IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>
The state type

`TMessage`<br/>
The data type of the behavior

Implements [IVisitable](../masstransit/ivisitable), [IProbeSite](../masstransit/iprobesite)

## Methods

### **Execute(BehaviorContext\<TSaga, TMessage\>)**

Execute the activity with the given behavior context

```csharp
Task Execute(BehaviorContext<TSaga, TMessage> context)
```

#### Parameters

`context` [BehaviorContext\<TSaga, TMessage\>](../masstransit/behaviorcontext-2)<br/>
The behavior context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
An awaitable task

### **Faulted\<TException\>(BehaviorExceptionContext\<TSaga, TMessage, TException\>)**

The exception path through the behavior allows activities to catch and handle exceptions

```csharp
Task Faulted<TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context)
```

#### Type Parameters

`TException`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, TMessage, TException\>](../masstransit/behaviorexceptioncontext-3)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
