---

title: StateMachineVisitor

---

# StateMachineVisitor

Namespace: MassTransit

```csharp
public interface StateMachineVisitor
```

## Methods

### **Visit(State, Action\<State\>)**

```csharp
void Visit(State state, Action<State> next)
```

#### Parameters

`state` [State](../masstransit/state)<br/>

`next` [Action\<State\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Visit(Event, Action\<Event\>)**

```csharp
void Visit(Event event, Action<Event> next)
```

#### Parameters

`event` [Event](../masstransit/event)<br/>

`next` [Action\<Event\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Visit\<TMessage\>(Event\<TMessage\>, Action\<Event\<TMessage\>\>)**

```csharp
void Visit<TMessage>(Event<TMessage> event, Action<Event<TMessage>> next)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`event` [Event\<TMessage\>](../masstransit/event-1)<br/>

`next` [Action\<Event\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Visit(IStateMachineActivity)**

```csharp
void Visit(IStateMachineActivity activity)
```

#### Parameters

`activity` [IStateMachineActivity](../masstransit/istatemachineactivity)<br/>

### **Visit\<T\>(IBehavior\<T\>)**

```csharp
void Visit<T>(IBehavior<T> behavior)
```

#### Type Parameters

`T`<br/>

#### Parameters

`behavior` [IBehavior\<T\>](../masstransit/ibehavior-1)<br/>

### **Visit\<T\>(IBehavior\<T\>, Action\<IBehavior\<T\>\>)**

```csharp
void Visit<T>(IBehavior<T> behavior, Action<IBehavior<T>> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`behavior` [IBehavior\<T\>](../masstransit/ibehavior-1)<br/>

`next` [Action\<IBehavior\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Visit\<T, TMessage\>(IBehavior\<T, TMessage\>)**

```csharp
void Visit<T, TMessage>(IBehavior<T, TMessage> behavior)
```

#### Type Parameters

`T`<br/>

`TMessage`<br/>

#### Parameters

`behavior` [IBehavior\<T, TMessage\>](../masstransit/ibehavior-2)<br/>

### **Visit\<T, TMessage\>(IBehavior\<T, TMessage\>, Action\<IBehavior\<T, TMessage\>\>)**

```csharp
void Visit<T, TMessage>(IBehavior<T, TMessage> behavior, Action<IBehavior<T, TMessage>> next)
```

#### Type Parameters

`T`<br/>

`TMessage`<br/>

#### Parameters

`behavior` [IBehavior\<T, TMessage\>](../masstransit/ibehavior-2)<br/>

`next` [Action\<IBehavior\<T, TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Visit(IStateMachineActivity, Action\<IStateMachineActivity\>)**

```csharp
void Visit(IStateMachineActivity activity, Action<IStateMachineActivity> next)
```

#### Parameters

`activity` [IStateMachineActivity](../masstransit/istatemachineactivity)<br/>

`next` [Action\<IStateMachineActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
