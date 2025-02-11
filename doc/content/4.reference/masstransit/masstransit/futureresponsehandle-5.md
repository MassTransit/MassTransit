---

title: FutureResponseHandle<TCommand, TResult, TFault, TRequest, TResponse>

---

# FutureResponseHandle\<TCommand, TResult, TFault, TRequest, TResponse\>

Namespace: MassTransit

```csharp
public interface FutureResponseHandle<TCommand, TResult, TFault, TRequest, TResponse> : FutureRequestHandle<TCommand, TResult, TFault, TRequest>
```

#### Type Parameters

`TCommand`<br/>

`TResult`<br/>

`TFault`<br/>

`TRequest`<br/>

`TResponse`<br/>

Implements [FutureRequestHandle\<TCommand, TResult, TFault, TRequest\>](../masstransit/futurerequesthandle-4)

## Properties

### **Completed**

The Response Completed event

```csharp
public abstract Event<TResponse> Completed { get; }
```

#### Property Value

[Event\<TResponse\>](../../masstransit-abstractions/masstransit/event-1)<br/>
