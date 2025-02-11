---

title: IFutureStateMachineConfigurator

---

# IFutureStateMachineConfigurator

Namespace: MassTransit.Futures

```csharp
public interface IFutureStateMachineConfigurator
```

## Methods

### **CreateResponseEvent\<T\>()**

```csharp
Event<T> CreateResponseEvent<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[Event\<T\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **SetResult\<T\>(Event\<T\>, Func\<BehaviorContext\<FutureState, T\>, Task\>)**

Set the Future's result to the specified value

```csharp
void SetResult<T>(Event<T> responseReceived, Func<BehaviorContext<FutureState, T>, Task> callback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`responseReceived` [Event\<T\>](../../masstransit-abstractions/masstransit/event-1)<br/>

`callback` [Func\<BehaviorContext\<FutureState, T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **SetFaulted\<T\>(Event\<T\>, Func\<BehaviorContext\<FutureState, T\>, Task\>)**

Set the Future to the Faulted state, and set the Fault message

```csharp
void SetFaulted<T>(Event<T> requestCompleted, Func<BehaviorContext<FutureState, T>, Task> callback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`requestCompleted` [Event\<T\>](../../masstransit-abstractions/masstransit/event-1)<br/>

`callback` [Func\<BehaviorContext\<FutureState, T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **CompletePendingRequest\<T\>(Event\<T\>, PendingFutureIdProvider\<T\>)**

Set the result for a pending request and remove the identifier

```csharp
void CompletePendingRequest<T>(Event<T> requestCompleted, PendingFutureIdProvider<T> pendingIdProvider)
```

#### Type Parameters

`T`<br/>

#### Parameters

`requestCompleted` [Event\<T\>](../../masstransit-abstractions/masstransit/event-1)<br/>

`pendingIdProvider` [PendingFutureIdProvider\<T\>](../masstransit/pendingfutureidprovider-1)<br/>

### **DuringAnyWhen\<T\>(Event\<T\>, Func\<EventActivityBinder\<FutureState, T\>, EventActivityBinder\<FutureState, T\>\>)**

Add an event handler to the future

```csharp
void DuringAnyWhen<T>(Event<T> whenEvent, Func<EventActivityBinder<FutureState, T>, EventActivityBinder<FutureState, T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`whenEvent` [Event\<T\>](../../masstransit-abstractions/masstransit/event-1)<br/>

`configure` [Func\<EventActivityBinder\<FutureState, T\>, EventActivityBinder\<FutureState, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
