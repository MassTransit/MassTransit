---

title: IBehavior<TInstance>

---

# IBehavior\<TInstance\>

Namespace: MassTransit

A behavior is a chain of activities invoked by a state

```csharp
public interface IBehavior<TInstance> : IVisitable, IProbeSite
```

#### Type Parameters

`TInstance`<br/>
The state type

Implements [IVisitable](../masstransit/ivisitable), [IProbeSite](../masstransit/iprobesite)

## Methods

### **Execute(BehaviorContext\<TInstance\>)**

Execute the activity with the given behavior context

```csharp
Task Execute(BehaviorContext<TInstance> context)
```

#### Parameters

`context` [BehaviorContext\<TInstance\>](../masstransit/behaviorcontext-1)<br/>
The behavior context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
An awaitable task

### **Execute\<T\>(BehaviorContext\<TInstance, T\>)**

Execute the activity with the given behavior context

```csharp
Task Execute<T>(BehaviorContext<TInstance, T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorContext\<TInstance, T\>](../masstransit/behaviorcontext-2)<br/>
The behavior context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
An awaitable task

### **Faulted\<T, TException\>(BehaviorExceptionContext\<TInstance, T, TException\>)**

The exception path through the behavior allows activities to catch and handle exceptions

```csharp
Task Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context)
```

#### Type Parameters

`T`<br/>

`TException`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TInstance, T, TException\>](../masstransit/behaviorexceptioncontext-3)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted\<TException\>(BehaviorExceptionContext\<TInstance, TException\>)**

The exception path through the behavior allows activities to catch and handle exceptions

```csharp
Task Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context)
```

#### Type Parameters

`TException`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TInstance, TException\>](../masstransit/behaviorexceptioncontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
