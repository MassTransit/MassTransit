---

title: RetryActivityBinder<TInstance, TMessage>

---

# RetryActivityBinder\<TInstance, TMessage\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class RetryActivityBinder<TInstance, TMessage> : IActivityBinder<TInstance>
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RetryActivityBinder\<TInstance, TMessage\>](../masstransit-sagastatemachine/retryactivitybinder-2)<br/>
Implements [IActivityBinder\<TInstance\>](../masstransit-sagastatemachine/iactivitybinder-1)

## Properties

### **Event**

```csharp
public Event Event { get; }
```

#### Property Value

[Event](../../masstransit-abstractions/masstransit/event)<br/>

## Constructors

### **RetryActivityBinder(Event, IRetryPolicy, EventActivities\<TInstance\>)**

```csharp
public RetryActivityBinder(Event event, IRetryPolicy retryPolicy, EventActivities<TInstance> retryActivities)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`retryPolicy` [IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

`retryActivities` [EventActivities\<TInstance\>](../masstransit/eventactivities-1)<br/>

## Methods

### **IsStateTransitionEvent(State)**

```csharp
public bool IsStateTransitionEvent(State state)
```

#### Parameters

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Bind(State\<TInstance\>)**

```csharp
public void Bind(State<TInstance> state)
```

#### Parameters

`state` [State\<TInstance\>](../../masstransit-abstractions/masstransit/state-1)<br/>

### **Bind(IBehaviorBuilder\<TInstance\>)**

```csharp
public void Bind(IBehaviorBuilder<TInstance> builder)
```

#### Parameters

`builder` [IBehaviorBuilder\<TInstance\>](../masstransit-sagastatemachine/ibehaviorbuilder-1)<br/>
