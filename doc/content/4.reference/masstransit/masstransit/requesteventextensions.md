---

title: RequestEventExtensions

---

# RequestEventExtensions

Namespace: MassTransit

```csharp
public static class RequestEventExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestEventExtensions](../masstransit/requesteventextensions)

## Methods

### **RequestStarted\<TInstance, TData\>(EventActivityBinder\<TInstance, TData\>)**

Publishes the  event, used by the request state machine to track
 pending requests for a saga instance.

```csharp
public static EventActivityBinder<TInstance, TData> RequestStarted<TInstance, TData>(EventActivityBinder<TInstance, TData> source)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **RequestCompleted\<TInstance, TData\>(EventActivityBinder\<TInstance, TData\>)**

Publishes the  event, used by the request state machine to complete pending
 requests. The response type of the inbound request must be the same as the  type.

```csharp
public static EventActivityBinder<TInstance, TData> RequestCompleted<TInstance, TData>(EventActivityBinder<TInstance, TData> source)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **RequestCompleted\<TInstance, TData, TResponse\>(EventActivityBinder\<TInstance, TData\>, AsyncEventMessageFactory\<TInstance, TData, TResponse\>)**

Publishes the  event, used by the request state machine to complete pending
 requests.

```csharp
public static EventActivityBinder<TInstance, TData> RequestCompleted<TInstance, TData, TResponse>(EventActivityBinder<TInstance, TData> source, AsyncEventMessageFactory<TInstance, TData, TResponse> messageFactory)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TResponse`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`messageFactory` [AsyncEventMessageFactory\<TInstance, TData, TResponse\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **RequestFaulted\<TInstance, TData, TRequest\>(EventActivityBinder\<TInstance, TData\>, Event\<TRequest\>)**

Publishes the  event, used by the request state machine to fault pending requests

```csharp
public static EventActivityBinder<TInstance, TData> RequestFaulted<TInstance, TData, TRequest>(EventActivityBinder<TInstance, TData> source, Event<TRequest> requestEvent)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TRequest`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`requestEvent` [Event\<TRequest\>](../../masstransit-abstractions/masstransit/event-1)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>
