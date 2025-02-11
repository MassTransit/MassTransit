---

title: RequestExtensions

---

# RequestExtensions

Namespace: MassTransit

```csharp
public static class RequestExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestExtensions](../masstransit/requestextensions)

## Methods

### **Request\<TRequest, TResponse\>(IBus, Uri, TRequest, CancellationToken, RequestTimeout, Action\<SendContext\<TRequest\>\>)**

Send a request from the bus to the endpoint, and return a Task which can be awaited for the response.

```csharp
public static Task<Response<TResponse>> Request<TRequest, TResponse>(IBus bus, Uri destinationAddress, TRequest message, CancellationToken cancellationToken, RequestTimeout timeout, Action<SendContext<TRequest>> callback)
```

#### Type Parameters

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>
A started bus instance

`destinationAddress` Uri<br/>
The service address

`message` TRequest<br/>
The request message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
An optional cancellationToken for this request

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>
An optional timeout for the request (defaults to 30 seconds)

`callback` [Action\<SendContext\<TRequest\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
A callback, which can modify the  of the request

#### Returns

[Task\<Response\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Request\<TRequest, TResponse\>(IBus, Uri, Object, CancellationToken, RequestTimeout, Action\<SendContext\<TRequest\>\>)**

Send a request from the bus to the endpoint, and return a Task which can be awaited for the response.

```csharp
public static Task<Response<TResponse>> Request<TRequest, TResponse>(IBus bus, Uri destinationAddress, object values, CancellationToken cancellationToken, RequestTimeout timeout, Action<SendContext<TRequest>> callback)
```

#### Type Parameters

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>
A started bus instance

`destinationAddress` Uri<br/>
The service address

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values used to initialize the request message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
An optional cancellationToken for this request

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>
An optional timeout for the request (defaults to 30 seconds)

`callback` [Action\<SendContext\<TRequest\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
A callback, which can modify the  of the request

#### Returns

[Task\<Response\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Request\<TRequest, TResponse\>(IBus, TRequest, CancellationToken, RequestTimeout, Action\<SendContext\<TRequest\>\>)**

Send a request from the bus to the endpoint, and return a Task which can be awaited for the response.

```csharp
public static Task<Response<TResponse>> Request<TRequest, TResponse>(IBus bus, TRequest message, CancellationToken cancellationToken, RequestTimeout timeout, Action<SendContext<TRequest>> callback)
```

#### Type Parameters

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>
A started bus instance

`message` TRequest<br/>
The request message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
An optional cancellationToken for this request

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>
An optional timeout for the request (defaults to 30 seconds)

`callback` [Action\<SendContext\<TRequest\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
A callback, which can modify the  of the request

#### Returns

[Task\<Response\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Request\<TRequest, TResponse\>(IBus, Object, CancellationToken, RequestTimeout, Action\<SendContext\<TRequest\>\>)**

Send a request from the bus to the endpoint, and return a Task which can be awaited for the response.

```csharp
public static Task<Response<TResponse>> Request<TRequest, TResponse>(IBus bus, object values, CancellationToken cancellationToken, RequestTimeout timeout, Action<SendContext<TRequest>> callback)
```

#### Type Parameters

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>
A started bus instance

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values used to initialize the request message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
An optional cancellationToken for this request

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>
An optional timeout for the request (defaults to 30 seconds)

`callback` [Action\<SendContext\<TRequest\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
A callback, which can modify the  of the request

#### Returns

[Task\<Response\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Request\<TRequest, TResponse\>(ConsumeContext, IBus, Uri, TRequest, CancellationToken, RequestTimeout, Action\<SendContext\<TRequest\>\>)**

Send a request from the bus to the endpoint, and return a Task which can be awaited for the response.

```csharp
public static Task<Response<TResponse>> Request<TRequest, TResponse>(ConsumeContext consumeContext, IBus bus, Uri destinationAddress, TRequest message, CancellationToken cancellationToken, RequestTimeout timeout, Action<SendContext<TRequest>> callback)
```

#### Type Parameters

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

#### Parameters

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>
A started bus instance

`destinationAddress` Uri<br/>
The service address

`message` TRequest<br/>
The request message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
An optional cancellationToken for this request

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>
An optional timeout for the request (defaults to 30 seconds)

`callback` [Action\<SendContext\<TRequest\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
A callback, which can modify the  of the request

#### Returns

[Task\<Response\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Request\<TRequest, TResponse\>(ConsumeContext, IBus, Uri, Object, CancellationToken, RequestTimeout, Action\<SendContext\<TRequest\>\>)**

Send a request from the bus to the endpoint, and return a Task which can be awaited for the response.

```csharp
public static Task<Response<TResponse>> Request<TRequest, TResponse>(ConsumeContext consumeContext, IBus bus, Uri destinationAddress, object values, CancellationToken cancellationToken, RequestTimeout timeout, Action<SendContext<TRequest>> callback)
```

#### Type Parameters

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

#### Parameters

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>
A started bus instance

`destinationAddress` Uri<br/>
The service address

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values used to initialize the request message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
An optional cancellationToken for this request

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>
An optional timeout for the request (defaults to 30 seconds)

`callback` [Action\<SendContext\<TRequest\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
A callback, which can modify the  of the request

#### Returns

[Task\<Response\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Request\<TRequest, TResponse\>(ConsumeContext, IBus, TRequest, CancellationToken, RequestTimeout, Action\<SendContext\<TRequest\>\>)**

Send a request from the bus to the endpoint, and return a Task which can be awaited for the response.

```csharp
public static Task<Response<TResponse>> Request<TRequest, TResponse>(ConsumeContext consumeContext, IBus bus, TRequest message, CancellationToken cancellationToken, RequestTimeout timeout, Action<SendContext<TRequest>> callback)
```

#### Type Parameters

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

#### Parameters

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>
A started bus instance

`message` TRequest<br/>
The request message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
An optional cancellationToken for this request

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>
An optional timeout for the request (defaults to 30 seconds)

`callback` [Action\<SendContext\<TRequest\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
A callback, which can modify the  of the request

#### Returns

[Task\<Response\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Request\<TRequest, TResponse\>(ConsumeContext, IBus, Object, CancellationToken, RequestTimeout, Action\<SendContext\<TRequest\>\>)**

Send a request from the bus to the endpoint, and return a Task which can be awaited for the response.

```csharp
public static Task<Response<TResponse>> Request<TRequest, TResponse>(ConsumeContext consumeContext, IBus bus, object values, CancellationToken cancellationToken, RequestTimeout timeout, Action<SendContext<TRequest>> callback)
```

#### Type Parameters

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

#### Parameters

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>
A started bus instance

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values used to initialize the request message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
An optional cancellationToken for this request

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>
An optional timeout for the request (defaults to 30 seconds)

`callback` [Action\<SendContext\<TRequest\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
A callback, which can modify the  of the request

#### Returns

[Task\<Response\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
