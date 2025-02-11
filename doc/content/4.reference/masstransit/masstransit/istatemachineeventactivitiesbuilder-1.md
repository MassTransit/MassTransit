---

title: IStateMachineEventActivitiesBuilder<TSaga>

---

# IStateMachineEventActivitiesBuilder\<TSaga\>

Namespace: MassTransit

```csharp
public interface IStateMachineEventActivitiesBuilder<TSaga> : IStateMachineModifier<TSaga>
```

#### Type Parameters

`TSaga`<br/>

Implements [IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)

## Properties

### **IsCommitted**

```csharp
public abstract bool IsCommitted { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **When(Event, Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>)**

```csharp
IStateMachineEventActivitiesBuilder<TSaga> When(Event event, Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> configure)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`configure` [Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IStateMachineEventActivitiesBuilder\<TSaga\>](../masstransit/istatemachineeventactivitiesbuilder-1)<br/>

### **When(Event, StateMachineCondition\<TSaga\>, Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>)**

```csharp
IStateMachineEventActivitiesBuilder<TSaga> When(Event event, StateMachineCondition<TSaga> filter, Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> configure)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`filter` [StateMachineCondition\<TSaga\>](../../masstransit-abstractions/masstransit/statemachinecondition-1)<br/>

`configure` [Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IStateMachineEventActivitiesBuilder\<TSaga\>](../masstransit/istatemachineeventactivitiesbuilder-1)<br/>

### **When\<TData\>(Event\<TData\>, Func\<EventActivityBinder\<TSaga, TData\>, EventActivityBinder\<TSaga, TData\>\>)**

```csharp
IStateMachineEventActivitiesBuilder<TSaga> When<TData>(Event<TData> event, Func<EventActivityBinder<TSaga, TData>, EventActivityBinder<TSaga, TData>> configure)
```

#### Type Parameters

`TData`<br/>

#### Parameters

`event` [Event\<TData\>](../../masstransit-abstractions/masstransit/event-1)<br/>

`configure` [Func\<EventActivityBinder\<TSaga, TData\>, EventActivityBinder\<TSaga, TData\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IStateMachineEventActivitiesBuilder\<TSaga\>](../masstransit/istatemachineeventactivitiesbuilder-1)<br/>

### **When\<TData\>(Event\<TData\>, StateMachineCondition\<TSaga, TData\>, Func\<EventActivityBinder\<TSaga, TData\>, EventActivityBinder\<TSaga, TData\>\>)**

```csharp
IStateMachineEventActivitiesBuilder<TSaga> When<TData>(Event<TData> event, StateMachineCondition<TSaga, TData> filter, Func<EventActivityBinder<TSaga, TData>, EventActivityBinder<TSaga, TData>> configure)
```

#### Type Parameters

`TData`<br/>

#### Parameters

`event` [Event\<TData\>](../../masstransit-abstractions/masstransit/event-1)<br/>

`filter` [StateMachineCondition\<TSaga, TData\>](../../masstransit-abstractions/masstransit/statemachinecondition-2)<br/>

`configure` [Func\<EventActivityBinder\<TSaga, TData\>, EventActivityBinder\<TSaga, TData\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IStateMachineEventActivitiesBuilder\<TSaga\>](../masstransit/istatemachineeventactivitiesbuilder-1)<br/>

### **Ignore(Event)**

```csharp
IStateMachineEventActivitiesBuilder<TSaga> Ignore(Event event)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

#### Returns

[IStateMachineEventActivitiesBuilder\<TSaga\>](../masstransit/istatemachineeventactivitiesbuilder-1)<br/>

### **Ignore\<TMessage\>(Event\<TMessage\>)**

```csharp
IStateMachineEventActivitiesBuilder<TSaga> Ignore<TMessage>(Event<TMessage> event)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`event` [Event\<TMessage\>](../../masstransit-abstractions/masstransit/event-1)<br/>

#### Returns

[IStateMachineEventActivitiesBuilder\<TSaga\>](../masstransit/istatemachineeventactivitiesbuilder-1)<br/>

### **Ignore\<TMessage\>(Event\<TMessage\>, StateMachineCondition\<TSaga, TMessage\>)**

```csharp
IStateMachineEventActivitiesBuilder<TSaga> Ignore<TMessage>(Event<TMessage> event, StateMachineCondition<TSaga, TMessage> filter)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`event` [Event\<TMessage\>](../../masstransit-abstractions/masstransit/event-1)<br/>

`filter` [StateMachineCondition\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/statemachinecondition-2)<br/>

#### Returns

[IStateMachineEventActivitiesBuilder\<TSaga\>](../masstransit/istatemachineeventactivitiesbuilder-1)<br/>

### **CommitActivities()**

```csharp
IStateMachineModifier<TSaga> CommitActivities()
```

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>
