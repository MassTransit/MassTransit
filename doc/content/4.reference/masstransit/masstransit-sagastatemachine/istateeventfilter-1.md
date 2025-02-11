---

title: IStateEventFilter<TSaga>

---

# IStateEventFilter\<TSaga\>

Namespace: MassTransit.SagaStateMachine

```csharp
public interface IStateEventFilter<TSaga>
```

#### Type Parameters

`TSaga`<br/>

## Methods

### **Filter(BehaviorContext\<TSaga\>)**

```csharp
bool Filter(BehaviorContext<TSaga> context)
```

#### Parameters

`context` [BehaviorContext\<TSaga\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Filter\<T\>(BehaviorContext\<TSaga, T\>)**

```csharp
bool Filter<T>(BehaviorContext<TSaga, T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorContext\<TSaga, T\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
