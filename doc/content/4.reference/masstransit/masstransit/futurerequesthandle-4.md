---

title: FutureRequestHandle<TCommand, TResult, TFault, TRequest>

---

# FutureRequestHandle\<TCommand, TResult, TFault, TRequest\>

Namespace: MassTransit

```csharp
public interface FutureRequestHandle<TCommand, TResult, TFault, TRequest>
```

#### Type Parameters

`TCommand`<br/>

`TResult`<br/>

`TFault`<br/>

`TRequest`<br/>

## Properties

### **Faulted**

The Request Faulted event

```csharp
public abstract Event<Fault<TRequest>> Faulted { get; }
```

#### Property Value

[Event\<Fault\<TRequest\>\>](../../masstransit-abstractions/masstransit/event-1)<br/>

## Methods

### **OnResponseReceived\<T\>(Action\<IFutureResponseConfigurator\<TResult, T\>\>)**

Handle the response type specified, and configure the response behavior

```csharp
FutureResponseHandle<TCommand, TResult, TFault, TRequest, T> OnResponseReceived<T>(Action<IFutureResponseConfigurator<TResult, T>> configure)
```

#### Type Parameters

`T`<br/>
The response type

#### Parameters

`configure` [Action\<IFutureResponseConfigurator\<TResult, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[FutureResponseHandle\<TCommand, TResult, TFault, TRequest, T\>](../masstransit/futureresponsehandle-5)<br/>
