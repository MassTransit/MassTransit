---

title: StateMachineRequestExtensions

---

# StateMachineRequestExtensions

Namespace: MassTransit

```csharp
public static class StateMachineRequestExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StateMachineRequestExtensions](../masstransit/statemachinerequestextensions)

## Methods

### **Request\<TInstance, TData, TRequest, TResponse\>(EventActivityBinder\<TInstance, TData\>, Request\<TInstance, TRequest, TResponse\>, EventMessageFactory\<TInstance, TData, TRequest\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static EventActivityBinder<TInstance, TData> Request<TInstance, TData, TRequest, TResponse>(EventActivityBinder<TInstance, TData> binder, Request<TInstance, TRequest, TResponse> request, EventMessageFactory<TInstance, TData, TRequest> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TData`<br/>
The event data type

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`messageFactory` [EventMessageFactory\<TInstance, TData, TRequest\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>
The request message factory

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Request\<TInstance, TData, TRequest, TResponse\>(EventActivityBinder\<TInstance, TData\>, Request\<TInstance, TRequest, TResponse\>, AsyncEventMessageFactory\<TInstance, TData, TRequest\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static EventActivityBinder<TInstance, TData> Request<TInstance, TData, TRequest, TResponse>(EventActivityBinder<TInstance, TData> binder, Request<TInstance, TRequest, TResponse> request, AsyncEventMessageFactory<TInstance, TData, TRequest> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TData`<br/>
The event data type

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`messageFactory` [AsyncEventMessageFactory\<TInstance, TData, TRequest\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>
The request message factory

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Request\<TInstance, TData, TRequest, TResponse\>(EventActivityBinder\<TInstance, TData\>, Request\<TInstance, TRequest, TResponse\>, Func\<BehaviorContext\<TInstance, TData\>, Task\<SendTuple\<TRequest\>\>\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static EventActivityBinder<TInstance, TData> Request<TInstance, TData, TRequest, TResponse>(EventActivityBinder<TInstance, TData> binder, Request<TInstance, TRequest, TResponse> request, Func<BehaviorContext<TInstance, TData>, Task<SendTuple<TRequest>>> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TData`<br/>
The event data type

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`messageFactory` [Func\<BehaviorContext\<TInstance, TData\>, Task\<SendTuple\<TRequest\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The request message factory

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Request\<TInstance, TData, TRequest, TResponse\>(EventActivityBinder\<TInstance, TData\>, Request\<TInstance, TRequest, TResponse\>, ServiceAddressProvider\<TInstance, TData\>, EventMessageFactory\<TInstance, TData, TRequest\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static EventActivityBinder<TInstance, TData> Request<TInstance, TData, TRequest, TResponse>(EventActivityBinder<TInstance, TData> binder, Request<TInstance, TRequest, TResponse> request, ServiceAddressProvider<TInstance, TData> serviceAddressProvider, EventMessageFactory<TInstance, TData, TRequest> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TData`<br/>
The event data type

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`serviceAddressProvider` [ServiceAddressProvider\<TInstance, TData\>](../../masstransit-abstractions/masstransit/serviceaddressprovider-2)<br/>
A provider for the address used for the request

`messageFactory` [EventMessageFactory\<TInstance, TData, TRequest\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>
The request message factory

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Request\<TInstance, TData, TRequest, TResponse\>(EventActivityBinder\<TInstance, TData\>, Request\<TInstance, TRequest, TResponse\>, ServiceAddressProvider\<TInstance, TData\>, AsyncEventMessageFactory\<TInstance, TData, TRequest\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static EventActivityBinder<TInstance, TData> Request<TInstance, TData, TRequest, TResponse>(EventActivityBinder<TInstance, TData> binder, Request<TInstance, TRequest, TResponse> request, ServiceAddressProvider<TInstance, TData> serviceAddressProvider, AsyncEventMessageFactory<TInstance, TData, TRequest> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TData`<br/>
The event data type

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`serviceAddressProvider` [ServiceAddressProvider\<TInstance, TData\>](../../masstransit-abstractions/masstransit/serviceaddressprovider-2)<br/>
A provider for the address used for the request

`messageFactory` [AsyncEventMessageFactory\<TInstance, TData, TRequest\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>
The request message factory

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Request\<TInstance, TData, TRequest, TResponse\>(EventActivityBinder\<TInstance, TData\>, Request\<TInstance, TRequest, TResponse\>, ServiceAddressProvider\<TInstance, TData\>, Func\<BehaviorContext\<TInstance, TData\>, Task\<SendTuple\<TRequest\>\>\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static EventActivityBinder<TInstance, TData> Request<TInstance, TData, TRequest, TResponse>(EventActivityBinder<TInstance, TData> binder, Request<TInstance, TRequest, TResponse> request, ServiceAddressProvider<TInstance, TData> serviceAddressProvider, Func<BehaviorContext<TInstance, TData>, Task<SendTuple<TRequest>>> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TData`<br/>
The event data type

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`serviceAddressProvider` [ServiceAddressProvider\<TInstance, TData\>](../../masstransit-abstractions/masstransit/serviceaddressprovider-2)<br/>
A provider for the address used for the request

`messageFactory` [Func\<BehaviorContext\<TInstance, TData\>, Task\<SendTuple\<TRequest\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The request message factory

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Request\<TInstance, TException, TRequest, TResponse\>(ExceptionActivityBinder\<TInstance, TException\>, Request\<TInstance, TRequest, TResponse\>, EventExceptionMessageFactory\<TInstance, TException, TRequest\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static ExceptionActivityBinder<TInstance, TException> Request<TInstance, TException, TRequest, TResponse>(ExceptionActivityBinder<TInstance, TException> binder, Request<TInstance, TRequest, TResponse> request, EventExceptionMessageFactory<TInstance, TException, TRequest> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TException`<br/>

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`messageFactory` [EventExceptionMessageFactory\<TInstance, TException, TRequest\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-3)<br/>
The request message factory

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Request\<TInstance, TException, TRequest, TResponse\>(ExceptionActivityBinder\<TInstance, TException\>, Request\<TInstance, TRequest, TResponse\>, AsyncEventExceptionMessageFactory\<TInstance, TException, TRequest\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static ExceptionActivityBinder<TInstance, TException> Request<TInstance, TException, TRequest, TResponse>(ExceptionActivityBinder<TInstance, TException> binder, Request<TInstance, TRequest, TResponse> request, AsyncEventExceptionMessageFactory<TInstance, TException, TRequest> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TException`<br/>

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`messageFactory` [AsyncEventExceptionMessageFactory\<TInstance, TException, TRequest\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-3)<br/>
The request message factory

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Request\<TInstance, TException, TRequest, TResponse\>(ExceptionActivityBinder\<TInstance, TException\>, Request\<TInstance, TRequest, TResponse\>, Func\<BehaviorExceptionContext\<TInstance, TException\>, Task\<SendTuple\<TRequest\>\>\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static ExceptionActivityBinder<TInstance, TException> Request<TInstance, TException, TRequest, TResponse>(ExceptionActivityBinder<TInstance, TException> binder, Request<TInstance, TRequest, TResponse> request, Func<BehaviorExceptionContext<TInstance, TException>, Task<SendTuple<TRequest>>> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TException`<br/>

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`messageFactory` [Func\<BehaviorExceptionContext\<TInstance, TException\>, Task\<SendTuple\<TRequest\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The request message factory

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Request\<TInstance, TException, TRequest, TResponse\>(ExceptionActivityBinder\<TInstance, TException\>, Request\<TInstance, TRequest, TResponse\>, ServiceAddressExceptionProvider\<TInstance, TException\>, EventExceptionMessageFactory\<TInstance, TException, TRequest\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static ExceptionActivityBinder<TInstance, TException> Request<TInstance, TException, TRequest, TResponse>(ExceptionActivityBinder<TInstance, TException> binder, Request<TInstance, TRequest, TResponse> request, ServiceAddressExceptionProvider<TInstance, TException> serviceAddressProvider, EventExceptionMessageFactory<TInstance, TException, TRequest> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TException`<br/>

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`serviceAddressProvider` [ServiceAddressExceptionProvider\<TInstance, TException\>](../../masstransit-abstractions/masstransit/serviceaddressexceptionprovider-2)<br/>

`messageFactory` [EventExceptionMessageFactory\<TInstance, TException, TRequest\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-3)<br/>
The request message factory

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Request\<TInstance, TException, TRequest, TResponse\>(ExceptionActivityBinder\<TInstance, TException\>, Request\<TInstance, TRequest, TResponse\>, ServiceAddressExceptionProvider\<TInstance, TException\>, AsyncEventExceptionMessageFactory\<TInstance, TException, TRequest\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static ExceptionActivityBinder<TInstance, TException> Request<TInstance, TException, TRequest, TResponse>(ExceptionActivityBinder<TInstance, TException> binder, Request<TInstance, TRequest, TResponse> request, ServiceAddressExceptionProvider<TInstance, TException> serviceAddressProvider, AsyncEventExceptionMessageFactory<TInstance, TException, TRequest> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TException`<br/>

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`serviceAddressProvider` [ServiceAddressExceptionProvider\<TInstance, TException\>](../../masstransit-abstractions/masstransit/serviceaddressexceptionprovider-2)<br/>

`messageFactory` [AsyncEventExceptionMessageFactory\<TInstance, TException, TRequest\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-3)<br/>
The request message factory

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Request\<TInstance, TException, TRequest, TResponse\>(ExceptionActivityBinder\<TInstance, TException\>, Request\<TInstance, TRequest, TResponse\>, ServiceAddressExceptionProvider\<TInstance, TException\>, Func\<BehaviorExceptionContext\<TInstance, TException\>, Task\<SendTuple\<TRequest\>\>\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static ExceptionActivityBinder<TInstance, TException> Request<TInstance, TException, TRequest, TResponse>(ExceptionActivityBinder<TInstance, TException> binder, Request<TInstance, TRequest, TResponse> request, ServiceAddressExceptionProvider<TInstance, TException> serviceAddressProvider, Func<BehaviorExceptionContext<TInstance, TException>, Task<SendTuple<TRequest>>> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TException`<br/>

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`serviceAddressProvider` [ServiceAddressExceptionProvider\<TInstance, TException\>](../../masstransit-abstractions/masstransit/serviceaddressexceptionprovider-2)<br/>

`messageFactory` [Func\<BehaviorExceptionContext\<TInstance, TException\>, Task\<SendTuple\<TRequest\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The request message factory

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Request\<TInstance, TData, TException, TRequest, TResponse\>(ExceptionActivityBinder\<TInstance, TData, TException\>, Request\<TInstance, TRequest, TResponse\>, EventExceptionMessageFactory\<TInstance, TData, TException, TRequest\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> Request<TInstance, TData, TException, TRequest, TResponse>(ExceptionActivityBinder<TInstance, TData, TException> binder, Request<TInstance, TRequest, TResponse> request, EventExceptionMessageFactory<TInstance, TData, TException, TRequest> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TData`<br/>
The event data type

`TException`<br/>

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`messageFactory` [EventExceptionMessageFactory\<TInstance, TData, TException, TRequest\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-4)<br/>
The request message factory

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Request\<TInstance, TData, TException, TRequest, TResponse\>(ExceptionActivityBinder\<TInstance, TData, TException\>, Request\<TInstance, TRequest, TResponse\>, AsyncEventExceptionMessageFactory\<TInstance, TData, TException, TRequest\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> Request<TInstance, TData, TException, TRequest, TResponse>(ExceptionActivityBinder<TInstance, TData, TException> binder, Request<TInstance, TRequest, TResponse> request, AsyncEventExceptionMessageFactory<TInstance, TData, TException, TRequest> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TData`<br/>
The event data type

`TException`<br/>

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`messageFactory` [AsyncEventExceptionMessageFactory\<TInstance, TData, TException, TRequest\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-4)<br/>
The request message factory

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Request\<TInstance, TData, TException, TRequest, TResponse\>(ExceptionActivityBinder\<TInstance, TData, TException\>, Request\<TInstance, TRequest, TResponse\>, Func\<BehaviorExceptionContext\<TInstance, TData, TException\>, Task\<SendTuple\<TRequest\>\>\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> Request<TInstance, TData, TException, TRequest, TResponse>(ExceptionActivityBinder<TInstance, TData, TException> binder, Request<TInstance, TRequest, TResponse> request, Func<BehaviorExceptionContext<TInstance, TData, TException>, Task<SendTuple<TRequest>>> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TData`<br/>
The event data type

`TException`<br/>

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`messageFactory` [Func\<BehaviorExceptionContext\<TInstance, TData, TException\>, Task\<SendTuple\<TRequest\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The request message factory

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Request\<TInstance, TData, TException, TRequest, TResponse\>(ExceptionActivityBinder\<TInstance, TData, TException\>, Request\<TInstance, TRequest, TResponse\>, ServiceAddressExceptionProvider\<TInstance, TData, TException\>, EventExceptionMessageFactory\<TInstance, TData, TException, TRequest\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> Request<TInstance, TData, TException, TRequest, TResponse>(ExceptionActivityBinder<TInstance, TData, TException> binder, Request<TInstance, TRequest, TResponse> request, ServiceAddressExceptionProvider<TInstance, TData, TException> serviceAddressProvider, EventExceptionMessageFactory<TInstance, TData, TException, TRequest> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TData`<br/>
The event data type

`TException`<br/>

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`serviceAddressProvider` [ServiceAddressExceptionProvider\<TInstance, TData, TException\>](../../masstransit-abstractions/masstransit/serviceaddressexceptionprovider-3)<br/>

`messageFactory` [EventExceptionMessageFactory\<TInstance, TData, TException, TRequest\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-4)<br/>
The request message factory

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Request\<TInstance, TData, TException, TRequest, TResponse\>(ExceptionActivityBinder\<TInstance, TData, TException\>, Request\<TInstance, TRequest, TResponse\>, ServiceAddressExceptionProvider\<TInstance, TData, TException\>, AsyncEventExceptionMessageFactory\<TInstance, TData, TException, TRequest\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> Request<TInstance, TData, TException, TRequest, TResponse>(ExceptionActivityBinder<TInstance, TData, TException> binder, Request<TInstance, TRequest, TResponse> request, ServiceAddressExceptionProvider<TInstance, TData, TException> serviceAddressProvider, AsyncEventExceptionMessageFactory<TInstance, TData, TException, TRequest> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TData`<br/>
The event data type

`TException`<br/>

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`serviceAddressProvider` [ServiceAddressExceptionProvider\<TInstance, TData, TException\>](../../masstransit-abstractions/masstransit/serviceaddressexceptionprovider-3)<br/>

`messageFactory` [AsyncEventExceptionMessageFactory\<TInstance, TData, TException, TRequest\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-4)<br/>
The request message factory

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Request\<TInstance, TData, TException, TRequest, TResponse\>(ExceptionActivityBinder\<TInstance, TData, TException\>, Request\<TInstance, TRequest, TResponse\>, ServiceAddressExceptionProvider\<TInstance, TData, TException\>, Func\<BehaviorExceptionContext\<TInstance, TData, TException\>, Task\<SendTuple\<TRequest\>\>\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> Request<TInstance, TData, TException, TRequest, TResponse>(ExceptionActivityBinder<TInstance, TData, TException> binder, Request<TInstance, TRequest, TResponse> request, ServiceAddressExceptionProvider<TInstance, TData, TException> serviceAddressProvider, Func<BehaviorExceptionContext<TInstance, TData, TException>, Task<SendTuple<TRequest>>> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TData`<br/>
The event data type

`TException`<br/>

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`serviceAddressProvider` [ServiceAddressExceptionProvider\<TInstance, TData, TException\>](../../masstransit-abstractions/masstransit/serviceaddressexceptionprovider-3)<br/>

`messageFactory` [Func\<BehaviorExceptionContext\<TInstance, TData, TException\>, Task\<SendTuple\<TRequest\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The request message factory

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Request\<TInstance, TRequest, TResponse\>(EventActivityBinder\<TInstance\>, Request\<TInstance, TRequest, TResponse\>, EventMessageFactory\<TInstance, TRequest\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static EventActivityBinder<TInstance> Request<TInstance, TRequest, TResponse>(EventActivityBinder<TInstance> binder, Request<TInstance, TRequest, TResponse> request, EventMessageFactory<TInstance, TRequest> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`messageFactory` [EventMessageFactory\<TInstance, TRequest\>](../../masstransit-abstractions/masstransit/eventmessagefactory-2)<br/>
The request message factory

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **Request\<TInstance, TRequest, TResponse\>(EventActivityBinder\<TInstance\>, Request\<TInstance, TRequest, TResponse\>, AsyncEventMessageFactory\<TInstance, TRequest\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static EventActivityBinder<TInstance> Request<TInstance, TRequest, TResponse>(EventActivityBinder<TInstance> binder, Request<TInstance, TRequest, TResponse> request, AsyncEventMessageFactory<TInstance, TRequest> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`messageFactory` [AsyncEventMessageFactory\<TInstance, TRequest\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-2)<br/>
The request message factory

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **Request\<TInstance, TRequest, TResponse\>(EventActivityBinder\<TInstance\>, Request\<TInstance, TRequest, TResponse\>, Func\<BehaviorContext\<TInstance\>, Task\<SendTuple\<TRequest\>\>\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static EventActivityBinder<TInstance> Request<TInstance, TRequest, TResponse>(EventActivityBinder<TInstance> binder, Request<TInstance, TRequest, TResponse> request, Func<BehaviorContext<TInstance>, Task<SendTuple<TRequest>>> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`messageFactory` [Func\<BehaviorContext\<TInstance\>, Task\<SendTuple\<TRequest\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The request message factory

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **Request\<TInstance, TRequest, TResponse\>(EventActivityBinder\<TInstance\>, Request\<TInstance, TRequest, TResponse\>, ServiceAddressProvider\<TInstance\>, EventMessageFactory\<TInstance, TRequest\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static EventActivityBinder<TInstance> Request<TInstance, TRequest, TResponse>(EventActivityBinder<TInstance> binder, Request<TInstance, TRequest, TResponse> request, ServiceAddressProvider<TInstance> serviceAddressProvider, EventMessageFactory<TInstance, TRequest> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`serviceAddressProvider` [ServiceAddressProvider\<TInstance\>](../../masstransit-abstractions/masstransit/serviceaddressprovider-1)<br/>

`messageFactory` [EventMessageFactory\<TInstance, TRequest\>](../../masstransit-abstractions/masstransit/eventmessagefactory-2)<br/>
The request message factory

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **Request\<TInstance, TRequest, TResponse\>(EventActivityBinder\<TInstance\>, Request\<TInstance, TRequest, TResponse\>, ServiceAddressProvider\<TInstance\>, AsyncEventMessageFactory\<TInstance, TRequest\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static EventActivityBinder<TInstance> Request<TInstance, TRequest, TResponse>(EventActivityBinder<TInstance> binder, Request<TInstance, TRequest, TResponse> request, ServiceAddressProvider<TInstance> serviceAddressProvider, AsyncEventMessageFactory<TInstance, TRequest> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`serviceAddressProvider` [ServiceAddressProvider\<TInstance\>](../../masstransit-abstractions/masstransit/serviceaddressprovider-1)<br/>

`messageFactory` [AsyncEventMessageFactory\<TInstance, TRequest\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-2)<br/>
The request message factory

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **Request\<TInstance, TRequest, TResponse\>(EventActivityBinder\<TInstance\>, Request\<TInstance, TRequest, TResponse\>, ServiceAddressProvider\<TInstance\>, Func\<BehaviorContext\<TInstance\>, Task\<SendTuple\<TRequest\>\>\>)**

Send a request to the configured service endpoint, and setup the state machine to accept the response.

```csharp
public static EventActivityBinder<TInstance> Request<TInstance, TRequest, TResponse>(EventActivityBinder<TInstance> binder, Request<TInstance, TRequest, TResponse> request, ServiceAddressProvider<TInstance> serviceAddressProvider, Func<BehaviorContext<TInstance>, Task<SendTuple<TRequest>>> messageFactory)
```

#### Type Parameters

`TInstance`<br/>
The state instance type

`TRequest`<br/>
The request message type

`TResponse`<br/>
The response message type

#### Parameters

`binder` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>
The event binder

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>
The configured request to use

`serviceAddressProvider` [ServiceAddressProvider\<TInstance\>](../../masstransit-abstractions/masstransit/serviceaddressprovider-1)<br/>

`messageFactory` [Func\<BehaviorContext\<TInstance\>, Task\<SendTuple\<TRequest\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The request message factory

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **CancelRequestTimeout\<TInstance, TData, TRequest, TResponse\>(EventActivityBinder\<TInstance, TData\>, Request\<TInstance, TRequest, TResponse\>, Boolean)**

Cancels the request timeout, and clears the request data from the state instance

```csharp
public static EventActivityBinder<TInstance, TData> CancelRequestTimeout<TInstance, TData, TRequest, TResponse>(EventActivityBinder<TInstance, TData> binder, Request<TInstance, TRequest, TResponse> request, bool completed)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TRequest`<br/>

`TResponse`<br/>

#### Parameters

`binder` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>

`completed` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>
