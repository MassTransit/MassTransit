---

title: AllStateEventFilter<TSaga>

---

# AllStateEventFilter\<TSaga\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class AllStateEventFilter<TSaga> : IStateEventFilter<TSaga>
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AllStateEventFilter\<TSaga\>](../masstransit-sagastatemachine/allstateeventfilter-1)<br/>
Implements [IStateEventFilter\<TSaga\>](../masstransit-sagastatemachine/istateeventfilter-1)

## Constructors

### **AllStateEventFilter()**

```csharp
public AllStateEventFilter()
```

## Methods

### **Filter(BehaviorContext\<TSaga\>)**

```csharp
public bool Filter(BehaviorContext<TSaga> context)
```

#### Parameters

`context` [BehaviorContext\<TSaga\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Filter\<T\>(BehaviorContext\<TSaga, T\>)**

```csharp
public bool Filter<T>(BehaviorContext<TSaga, T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorContext\<TSaga, T\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
