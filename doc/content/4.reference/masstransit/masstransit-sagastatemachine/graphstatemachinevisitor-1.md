---

title: GraphStateMachineVisitor<TSaga>

---

# GraphStateMachineVisitor\<TSaga\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class GraphStateMachineVisitor<TSaga> : StateMachineVisitor
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [GraphStateMachineVisitor\<TSaga\>](../masstransit-sagastatemachine/graphstatemachinevisitor-1)<br/>
Implements [StateMachineVisitor](../../masstransit-abstractions/masstransit/statemachinevisitor)

## Properties

### **Graph**

```csharp
public StateMachineGraph Graph { get; }
```

#### Property Value

[StateMachineGraph](../masstransit-sagastatemachine/statemachinegraph)<br/>

## Constructors

### **GraphStateMachineVisitor(StateMachine\<TSaga\>)**

```csharp
public GraphStateMachineVisitor(StateMachine<TSaga> machine)
```

#### Parameters

`machine` [StateMachine\<TSaga\>](../../masstransit-abstractions/masstransit/statemachine-1)<br/>

## Methods

### **Visit(State, Action\<State\>)**

```csharp
public void Visit(State state, Action<State> next)
```

#### Parameters

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>

`next` [Action\<State\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Visit(Event, Action\<Event\>)**

```csharp
public void Visit(Event event, Action<Event> next)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`next` [Action\<Event\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Visit\<TData\>(Event\<TData\>, Action\<Event\<TData\>\>)**

```csharp
public void Visit<TData>(Event<TData> event, Action<Event<TData>> next)
```

#### Type Parameters

`TData`<br/>

#### Parameters

`event` [Event\<TData\>](../../masstransit-abstractions/masstransit/event-1)<br/>

`next` [Action\<Event\<TData\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Visit(IStateMachineActivity)**

```csharp
public void Visit(IStateMachineActivity activity)
```

#### Parameters

`activity` [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity)<br/>

### **Visit\<T\>(IBehavior\<T\>)**

```csharp
public void Visit<T>(IBehavior<T> behavior)
```

#### Type Parameters

`T`<br/>

#### Parameters

`behavior` [IBehavior\<T\>](../../masstransit-abstractions/masstransit/ibehavior-1)<br/>

### **Visit\<T\>(IBehavior\<T\>, Action\<IBehavior\<T\>\>)**

```csharp
public void Visit<T>(IBehavior<T> behavior, Action<IBehavior<T>> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`behavior` [IBehavior\<T\>](../../masstransit-abstractions/masstransit/ibehavior-1)<br/>

`next` [Action\<IBehavior\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Visit\<T, TData\>(IBehavior\<T, TData\>)**

```csharp
public void Visit<T, TData>(IBehavior<T, TData> behavior)
```

#### Type Parameters

`T`<br/>

`TData`<br/>

#### Parameters

`behavior` [IBehavior\<T, TData\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

### **Visit\<T, TData\>(IBehavior\<T, TData\>, Action\<IBehavior\<T, TData\>\>)**

```csharp
public void Visit<T, TData>(IBehavior<T, TData> behavior, Action<IBehavior<T, TData>> next)
```

#### Type Parameters

`T`<br/>

`TData`<br/>

#### Parameters

`behavior` [IBehavior\<T, TData\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

`next` [Action\<IBehavior\<T, TData\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Visit(IStateMachineActivity, Action\<IStateMachineActivity\>)**

```csharp
public void Visit(IStateMachineActivity activity, Action<IStateMachineActivity> next)
```

#### Parameters

`activity` [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity)<br/>

`next` [Action\<IStateMachineActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
