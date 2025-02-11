---

title: FutureExtensions

---

# FutureExtensions

Namespace: MassTransit

```csharp
public static class FutureExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureExtensions](../masstransit/futureextensions)

## Methods

### **InitializeFuture\<T\>(EventActivityBinder\<FutureState, T\>)**

Initialize the FutureState properties of the request

```csharp
public static EventActivityBinder<FutureState, T> InitializeFuture<T>(EventActivityBinder<FutureState, T> binder)
```

#### Type Parameters

`T`<br/>

#### Parameters

`binder` [EventActivityBinder\<FutureState, T\>](../masstransit/eventactivitybinder-2)<br/>

#### Returns

[EventActivityBinder\<FutureState, T\>](../masstransit/eventactivitybinder-2)<br/>

### **AddSubscription\<T\>(EventActivityBinder\<FutureState, T\>)**

Use when a request is received after the initial request is still awaiting completion

```csharp
public static EventActivityBinder<FutureState, T> AddSubscription<T>(EventActivityBinder<FutureState, T> binder)
```

#### Type Parameters

`T`<br/>

#### Parameters

`binder` [EventActivityBinder\<FutureState, T\>](../masstransit/eventactivitybinder-2)<br/>

#### Returns

[EventActivityBinder\<FutureState, T\>](../masstransit/eventactivitybinder-2)<br/>

### **SetResult\<T, TResult\>(EventActivityBinder\<FutureState, T\>, Func\<BehaviorContext\<FutureState, T\>, Guid\>, AsyncEventMessageFactory\<FutureState, T, TResult\>)**

Set the result associated with the identifier using the message factory

```csharp
public static EventActivityBinder<FutureState, T> SetResult<T, TResult>(EventActivityBinder<FutureState, T> binder, Func<BehaviorContext<FutureState, T>, Guid> getResultId, AsyncEventMessageFactory<FutureState, T, TResult> messageFactory)
```

#### Type Parameters

`T`<br/>
The event type

`TResult`<br/>
The result type

#### Parameters

`binder` [EventActivityBinder\<FutureState, T\>](../masstransit/eventactivitybinder-2)<br/>

`getResultId` [Func\<BehaviorContext\<FutureState, T\>, Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Should return the result identifier

`messageFactory` [AsyncEventMessageFactory\<FutureState, T, TResult\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>
Should return the result message

#### Returns

[EventActivityBinder\<FutureState, T\>](../masstransit/eventactivitybinder-2)<br/>

### **SetResult\<T, TResult\>(EventActivityBinder\<FutureState, T\>, Func\<BehaviorContext\<FutureState, T\>, Guid\>, EventMessageFactory\<FutureState, T, TResult\>)**

Set the result associated with the identifier using the message factory

```csharp
public static EventActivityBinder<FutureState, T> SetResult<T, TResult>(EventActivityBinder<FutureState, T> binder, Func<BehaviorContext<FutureState, T>, Guid> getResultId, EventMessageFactory<FutureState, T, TResult> messageFactory)
```

#### Type Parameters

`T`<br/>
The event type

`TResult`<br/>
The result type

#### Parameters

`binder` [EventActivityBinder\<FutureState, T\>](../masstransit/eventactivitybinder-2)<br/>

`getResultId` [Func\<BehaviorContext\<FutureState, T\>, Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Should return the result identifier

`messageFactory` [EventMessageFactory\<FutureState, T, TResult\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>
Should return the result message

#### Returns

[EventActivityBinder\<FutureState, T\>](../masstransit/eventactivitybinder-2)<br/>

### **SetFault\<T, TResult\>(EventActivityBinder\<FutureState, Fault\<T\>\>, Func\<BehaviorContext\<FutureState, Fault\<T\>\>, Guid\>, EventMessageFactory\<FutureState, Fault\<T\>, TResult\>)**

Set the result associated with the identifier using the message factory

```csharp
public static EventActivityBinder<FutureState, Fault<T>> SetFault<T, TResult>(EventActivityBinder<FutureState, Fault<T>> binder, Func<BehaviorContext<FutureState, Fault<T>>, Guid> getResultId, EventMessageFactory<FutureState, Fault<T>, TResult> messageFactory)
```

#### Type Parameters

`T`<br/>
The event type

`TResult`<br/>
The result type

#### Parameters

`binder` [EventActivityBinder\<FutureState, Fault\<T\>\>](../masstransit/eventactivitybinder-2)<br/>

`getResultId` [Func\<BehaviorContext\<FutureState, Fault\<T\>\>, Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Should return the result identifier

`messageFactory` [EventMessageFactory\<FutureState, Fault\<T\>, TResult\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>
Should return the result message

#### Returns

[EventActivityBinder\<FutureState, Fault\<T\>\>](../masstransit/eventactivitybinder-2)<br/>

### **SetFault\<TResult\>(EventActivityBinder\<FutureState, RoutingSlipFaulted\>, EventMessageFactory\<FutureState, RoutingSlipFaulted, TResult\>)**

Set the result associated with the identifier using the message factory

```csharp
public static EventActivityBinder<FutureState, RoutingSlipFaulted> SetFault<TResult>(EventActivityBinder<FutureState, RoutingSlipFaulted> binder, EventMessageFactory<FutureState, RoutingSlipFaulted, TResult> messageFactory)
```

#### Type Parameters

`TResult`<br/>
The result type

#### Parameters

`binder` [EventActivityBinder\<FutureState, RoutingSlipFaulted\>](../masstransit/eventactivitybinder-2)<br/>

`messageFactory` [EventMessageFactory\<FutureState, RoutingSlipFaulted, TResult\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>
Should return the result message

#### Returns

[EventActivityBinder\<FutureState, RoutingSlipFaulted\>](../masstransit/eventactivitybinder-2)<br/>
