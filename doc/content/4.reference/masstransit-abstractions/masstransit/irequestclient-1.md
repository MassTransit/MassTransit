---

title: IRequestClient<TRequest>

---

# IRequestClient\<TRequest\>

Namespace: MassTransit

A request client, which is used to send a request, as well as get one or more response types from that request.

```csharp
public interface IRequestClient<TRequest>
```

#### Type Parameters

`TRequest`<br/>
The request type

## Methods

### **Create(TRequest, CancellationToken, RequestTimeout)**

Create a request, returning a [RequestHandle\<TRequest\>](../masstransit/requesthandle-1), which is then used to get responses, and ultimately
 send the request.

```csharp
RequestHandle<TRequest> Create(TRequest message, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Parameters

`message` TRequest<br/>
The request message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
An optional cancellationToken to cancel the request

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>
An optional timeout, to automatically cancel the request after the specified timeout period

#### Returns

[RequestHandle\<TRequest\>](../masstransit/requesthandle-1)<br/>
A [RequestHandle\<TRequest\>](../masstransit/requesthandle-1) for the request

### **Create(Object, CancellationToken, RequestTimeout)**

Create a request, returning a [RequestHandle\<TRequest\>](../masstransit/requesthandle-1), which is then used to get responses, and ultimately
 send the request.

```csharp
RequestHandle<TRequest> Create(object values, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values to initialize the message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
An optional cancellationToken to cancel the request

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>
An optional timeout, to automatically cancel the request after the specified timeout period

#### Returns

[RequestHandle\<TRequest\>](../masstransit/requesthandle-1)<br/>
A [RequestHandle\<TRequest\>](../masstransit/requesthandle-1) for the request

### **GetResponse\<T\>(TRequest, CancellationToken, RequestTimeout)**

Create a request, and return a task for the specified response type

```csharp
Task<Response<T>> GetResponse<T>(TRequest message, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` TRequest<br/>
The request message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
An optional cancellationToken to cancel the request

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>
An optional timeout, to automatically cancel the request after the specified timeout period

#### Returns

[Task\<Response\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponse\<T\>(TRequest, RequestPipeConfiguratorCallback\<TRequest\>, CancellationToken, RequestTimeout)**

Create a request, and return a task for the specified response type

```csharp
Task<Response<T>> GetResponse<T>(TRequest message, RequestPipeConfiguratorCallback<TRequest> callback, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` TRequest<br/>
The request message

`callback` [RequestPipeConfiguratorCallback\<TRequest\>](../masstransit/requestpipeconfiguratorcallback-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
An optional cancellationToken to cancel the request

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>
An optional timeout, to automatically cancel the request after the specified timeout period

#### Returns

[Task\<Response\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponse\<T\>(Object, CancellationToken, RequestTimeout)**

Create a request, and return a task for the specified response type

```csharp
Task<Response<T>> GetResponse<T>(object values, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values to initialize the message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
An optional cancellationToken to cancel the request

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>
An optional timeout, to automatically cancel the request after the specified timeout period

#### Returns

[Task\<Response\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponse\<T\>(Object, RequestPipeConfiguratorCallback\<TRequest\>, CancellationToken, RequestTimeout)**

Create a request, and return a task for the specified response type

```csharp
Task<Response<T>> GetResponse<T>(object values, RequestPipeConfiguratorCallback<TRequest> callback, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values to initialize the message

`callback` [RequestPipeConfiguratorCallback\<TRequest\>](../masstransit/requestpipeconfiguratorcallback-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
An optional cancellationToken to cancel the request

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>
An optional timeout, to automatically cancel the request after the specified timeout period

#### Returns

[Task\<Response\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponse\<T1, T2\>(TRequest, CancellationToken, RequestTimeout)**

Create a request, and return a task for the specified response types

```csharp
Task<Response<T1, T2>> GetResponse<T1, T2>(TRequest message, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T1`<br/>
The first response type

`T2`<br/>
The second response type

#### Parameters

`message` TRequest<br/>
The request message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
An optional cancellationToken to cancel the request

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>
An optional timeout, to automatically cancel the request after the specified timeout period

#### Returns

[Task\<Response\<T1, T2\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponse\<T1, T2\>(TRequest, RequestPipeConfiguratorCallback\<TRequest\>, CancellationToken, RequestTimeout)**

Create a request, and return a task for the specified response types

```csharp
Task<Response<T1, T2>> GetResponse<T1, T2>(TRequest message, RequestPipeConfiguratorCallback<TRequest> callback, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T1`<br/>
The first response type

`T2`<br/>
The second response type

#### Parameters

`message` TRequest<br/>
The request message

`callback` [RequestPipeConfiguratorCallback\<TRequest\>](../masstransit/requestpipeconfiguratorcallback-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
An optional cancellationToken to cancel the request

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>
An optional timeout, to automatically cancel the request after the specified timeout period

#### Returns

[Task\<Response\<T1, T2\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponse\<T1, T2\>(Object, CancellationToken, RequestTimeout)**

Create a request, and return a task for the specified response types

```csharp
Task<Response<T1, T2>> GetResponse<T1, T2>(object values, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T1`<br/>
The first response type

`T2`<br/>
The second response type

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values to initialize the message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
An optional cancellationToken to cancel the request

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>
An optional timeout, to automatically cancel the request after the specified timeout period

#### Returns

[Task\<Response\<T1, T2\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponse\<T1, T2\>(Object, RequestPipeConfiguratorCallback\<TRequest\>, CancellationToken, RequestTimeout)**

Create a request, and return a task for the specified response types

```csharp
Task<Response<T1, T2>> GetResponse<T1, T2>(object values, RequestPipeConfiguratorCallback<TRequest> callback, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T1`<br/>
The first response type

`T2`<br/>
The second response type

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values to initialize the message

`callback` [RequestPipeConfiguratorCallback\<TRequest\>](../masstransit/requestpipeconfiguratorcallback-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
An optional cancellationToken to cancel the request

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>
An optional timeout, to automatically cancel the request after the specified timeout period

#### Returns

[Task\<Response\<T1, T2\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponse\<T1, T2, T3\>(TRequest, CancellationToken, RequestTimeout)**

Create a request, and return a task for the specified response types

```csharp
Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(TRequest message, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T1`<br/>
The first response type

`T2`<br/>
The second response type

`T3`<br/>

#### Parameters

`message` TRequest<br/>
The request message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
An optional cancellationToken to cancel the request

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>
An optional timeout, to automatically cancel the request after the specified timeout period

#### Returns

[Task\<Response\<T1, T2, T3\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponse\<T1, T2, T3\>(TRequest, RequestPipeConfiguratorCallback\<TRequest\>, CancellationToken, RequestTimeout)**

Create a request, and return a task for the specified response types

```csharp
Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(TRequest message, RequestPipeConfiguratorCallback<TRequest> callback, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T1`<br/>
The first response type

`T2`<br/>
The second response type

`T3`<br/>

#### Parameters

`message` TRequest<br/>
The request message

`callback` [RequestPipeConfiguratorCallback\<TRequest\>](../masstransit/requestpipeconfiguratorcallback-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
An optional cancellationToken to cancel the request

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>
An optional timeout, to automatically cancel the request after the specified timeout period

#### Returns

[Task\<Response\<T1, T2, T3\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponse\<T1, T2, T3\>(Object, CancellationToken, RequestTimeout)**

Create a request, and return a task for the specified response types

```csharp
Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(object values, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T1`<br/>
The first response type

`T2`<br/>
The second response type

`T3`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values to initialize the message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
An optional cancellationToken to cancel the request

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>
An optional timeout, to automatically cancel the request after the specified timeout period

#### Returns

[Task\<Response\<T1, T2, T3\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponse\<T1, T2, T3\>(Object, RequestPipeConfiguratorCallback\<TRequest\>, CancellationToken, RequestTimeout)**

Create a request, and return a task for the specified response types

```csharp
Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(object values, RequestPipeConfiguratorCallback<TRequest> callback, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T1`<br/>
The first response type

`T2`<br/>
The second response type

`T3`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values to initialize the message

`callback` [RequestPipeConfiguratorCallback\<TRequest\>](../masstransit/requestpipeconfiguratorcallback-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
An optional cancellationToken to cancel the request

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>
An optional timeout, to automatically cancel the request after the specified timeout period

#### Returns

[Task\<Response\<T1, T2, T3\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
