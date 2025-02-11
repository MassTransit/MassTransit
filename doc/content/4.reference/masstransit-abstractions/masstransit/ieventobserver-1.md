---

title: IEventObserver<TSaga>

---

# IEventObserver\<TSaga\>

Namespace: MassTransit

```csharp
public interface IEventObserver<TSaga>
```

#### Type Parameters

`TSaga`<br/>

## Methods

### **PreExecute(BehaviorContext\<TSaga\>)**

Called before the event context is delivered to the activities

```csharp
Task PreExecute(BehaviorContext<TSaga> context)
```

#### Parameters

`context` [BehaviorContext\<TSaga\>](../masstransit/behaviorcontext-1)<br/>
The event context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PreExecute\<T\>(BehaviorContext\<TSaga, T\>)**

Called before the event context is delivered to the activities

```csharp
Task PreExecute<T>(BehaviorContext<TSaga, T> context)
```

#### Type Parameters

`T`<br/>
The event data type

#### Parameters

`context` [BehaviorContext\<TSaga, T\>](../masstransit/behaviorcontext-2)<br/>
The event context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostExecute(BehaviorContext\<TSaga\>)**

Called when the event has been processed by the activities

```csharp
Task PostExecute(BehaviorContext<TSaga> context)
```

#### Parameters

`context` [BehaviorContext\<TSaga\>](../masstransit/behaviorcontext-1)<br/>
The event context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostExecute\<T\>(BehaviorContext\<TSaga, T\>)**

Called when the event has been processed by the activities

```csharp
Task PostExecute<T>(BehaviorContext<TSaga, T> context)
```

#### Type Parameters

`T`<br/>
The event data type

#### Parameters

`context` [BehaviorContext\<TSaga, T\>](../masstransit/behaviorcontext-2)<br/>
The event context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ExecuteFault(BehaviorContext\<TSaga\>, Exception)**

Called when the activity execution faults and is not handled by the activities

```csharp
Task ExecuteFault(BehaviorContext<TSaga> context, Exception exception)
```

#### Parameters

`context` [BehaviorContext\<TSaga\>](../masstransit/behaviorcontext-1)<br/>
The event context

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
The exception that was thrown

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ExecuteFault\<T\>(BehaviorContext\<TSaga, T\>, Exception)**

Called when the activity execution faults and is not handled by the activities

```csharp
Task ExecuteFault<T>(BehaviorContext<TSaga, T> context, Exception exception)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [BehaviorContext\<TSaga, T\>](../masstransit/behaviorcontext-2)<br/>
The event context

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
The exception that was thrown

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
