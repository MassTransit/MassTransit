---

title: IStateAccessor<TSaga>

---

# IStateAccessor\<TSaga\>

Namespace: MassTransit

```csharp
public interface IStateAccessor<TSaga> : IProbeSite
```

#### Type Parameters

`TSaga`<br/>

Implements [IProbeSite](../masstransit/iprobesite)

## Methods

### **Get(BehaviorContext\<TSaga\>)**

```csharp
Task<State<TSaga>> Get(BehaviorContext<TSaga> context)
```

#### Parameters

`context` [BehaviorContext\<TSaga\>](../masstransit/behaviorcontext-1)<br/>

#### Returns

[Task\<State\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Set(BehaviorContext\<TSaga\>, State\<TSaga\>)**

```csharp
Task Set(BehaviorContext<TSaga> context, State<TSaga> state)
```

#### Parameters

`context` [BehaviorContext\<TSaga\>](../masstransit/behaviorcontext-1)<br/>

`state` [State\<TSaga\>](../masstransit/state-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **GetStateExpression(State[])**

Converts a state expression to the instance current state property type.

```csharp
Expression<Func<TSaga, bool>> GetStateExpression(State[] states)
```

#### Parameters

`states` [State[]](../masstransit/state)<br/>

#### Returns

Expression\<Func\<TSaga, Boolean\>\><br/>
