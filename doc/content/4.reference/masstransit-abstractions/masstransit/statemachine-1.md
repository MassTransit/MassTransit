---

title: StateMachine<TSaga>

---

# StateMachine\<TSaga\>

Namespace: MassTransit

A defined state machine that operations against the specified instance

```csharp
public interface StateMachine<TSaga> : StateMachine, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

Implements [StateMachine](../masstransit/statemachine), [IVisitable](../masstransit/ivisitable), [IProbeSite](../masstransit/iprobesite)

## Properties

### **Accessor**

Exposes the current state on the given instance

```csharp
public abstract IStateAccessor<TSaga> Accessor { get; }
```

#### Property Value

[IStateAccessor\<TSaga\>](../masstransit/istateaccessor-1)<br/>

## Methods

### **GetState(String)**

Returns the state requested bound to the instance

```csharp
State<TSaga> GetState(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[State\<TSaga\>](../masstransit/state-1)<br/>

### **RaiseEvent(BehaviorContext\<TSaga\>)**

Raise a simple event on the state machine instance asynchronously

```csharp
Task RaiseEvent(BehaviorContext<TSaga> context)
```

#### Parameters

`context` [BehaviorContext\<TSaga\>](../masstransit/behaviorcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
Task for the instance once completed

### **RaiseEvent\<T\>(BehaviorContext\<TSaga, T\>)**

Raise a data event on the state machine instance

```csharp
Task RaiseEvent<T>(BehaviorContext<TSaga, T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorContext\<TSaga, T\>](../masstransit/behaviorcontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ConnectEventObserver(IEventObserver\<TSaga\>)**

```csharp
IDisposable ConnectEventObserver(IEventObserver<TSaga> observer)
```

#### Parameters

`observer` [IEventObserver\<TSaga\>](../masstransit/ieventobserver-1)<br/>

#### Returns

[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>

### **ConnectEventObserver(Event, IEventObserver\<TSaga\>)**

```csharp
IDisposable ConnectEventObserver(Event event, IEventObserver<TSaga> observer)
```

#### Parameters

`event` [Event](../masstransit/event)<br/>

`observer` [IEventObserver\<TSaga\>](../masstransit/ieventobserver-1)<br/>

#### Returns

[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>

### **ConnectStateObserver(IStateObserver\<TSaga\>)**

```csharp
IDisposable ConnectStateObserver(IStateObserver<TSaga> observer)
```

#### Parameters

`observer` [IStateObserver\<TSaga\>](../masstransit/istateobserver-1)<br/>

#### Returns

[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>
