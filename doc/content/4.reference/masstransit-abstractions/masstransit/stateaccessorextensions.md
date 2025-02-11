---

title: StateAccessorExtensions

---

# StateAccessorExtensions

Namespace: MassTransit

```csharp
public static class StateAccessorExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StateAccessorExtensions](../masstransit/stateaccessorextensions)

## Methods

### **GetState\<TSaga\>(IStateAccessor\<TSaga\>, BehaviorContext\<TSaga\>)**

```csharp
public static Task<State<TSaga>> GetState<TSaga>(IStateAccessor<TSaga> accessor, BehaviorContext<TSaga> context)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`accessor` [IStateAccessor\<TSaga\>](../masstransit/istateaccessor-1)<br/>

`context` [BehaviorContext\<TSaga\>](../masstransit/behaviorcontext-1)<br/>

#### Returns

[Task\<State\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetState\<TSaga\>(StateMachine\<TSaga\>, BehaviorContext\<TSaga\>)**

```csharp
public static Task<State<TSaga>> GetState<TSaga>(StateMachine<TSaga> accessor, BehaviorContext<TSaga> context)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`accessor` [StateMachine\<TSaga\>](../masstransit/statemachine-1)<br/>

`context` [BehaviorContext\<TSaga\>](../masstransit/behaviorcontext-1)<br/>

#### Returns

[Task\<State\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
