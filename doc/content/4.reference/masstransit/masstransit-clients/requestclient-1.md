---

title: RequestClient<TRequest>

---

# RequestClient\<TRequest\>

Namespace: MassTransit.Clients

```csharp
public class RequestClient<TRequest> : IRequestClient<TRequest>
```

#### Type Parameters

`TRequest`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestClient\<TRequest\>](../masstransit-clients/requestclient-1)<br/>
Implements [IRequestClient\<TRequest\>](../../masstransit-abstractions/masstransit/irequestclient-1)

## Constructors

### **RequestClient(ClientFactoryContext, IRequestSendEndpoint\<TRequest\>, RequestTimeout)**

```csharp
public RequestClient(ClientFactoryContext context, IRequestSendEndpoint<TRequest> requestSendEndpoint, RequestTimeout timeout)
```

#### Parameters

`context` [ClientFactoryContext](../../masstransit-abstractions/masstransit/clientfactorycontext)<br/>

`requestSendEndpoint` [IRequestSendEndpoint\<TRequest\>](../../masstransit-abstractions/masstransit/irequestsendendpoint-1)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

## Methods

### **Create(TRequest, CancellationToken, RequestTimeout)**

```csharp
public RequestHandle<TRequest> Create(TRequest message, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Parameters

`message` TRequest<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[RequestHandle\<TRequest\>](../../masstransit-abstractions/masstransit/requesthandle-1)<br/>

### **Create(Object, CancellationToken, RequestTimeout)**

```csharp
public RequestHandle<TRequest> Create(object values, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[RequestHandle\<TRequest\>](../../masstransit-abstractions/masstransit/requesthandle-1)<br/>

### **GetResponse\<T\>(TRequest, CancellationToken, RequestTimeout)**

```csharp
public Task<Response<T>> GetResponse<T>(TRequest message, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` TRequest<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[Task\<Response\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponse\<T\>(TRequest, RequestPipeConfiguratorCallback\<TRequest\>, CancellationToken, RequestTimeout)**

```csharp
public Task<Response<T>> GetResponse<T>(TRequest message, RequestPipeConfiguratorCallback<TRequest> callback, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` TRequest<br/>

`callback` [RequestPipeConfiguratorCallback\<TRequest\>](../../masstransit-abstractions/masstransit/requestpipeconfiguratorcallback-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[Task\<Response\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponse\<T\>(Object, CancellationToken, RequestTimeout)**

```csharp
public Task<Response<T>> GetResponse<T>(object values, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[Task\<Response\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponse\<T\>(Object, RequestPipeConfiguratorCallback\<TRequest\>, CancellationToken, RequestTimeout)**

```csharp
public Task<Response<T>> GetResponse<T>(object values, RequestPipeConfiguratorCallback<TRequest> callback, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`callback` [RequestPipeConfiguratorCallback\<TRequest\>](../../masstransit-abstractions/masstransit/requestpipeconfiguratorcallback-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[Task\<Response\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponse\<T1, T2\>(TRequest, CancellationToken, RequestTimeout)**

```csharp
public Task<Response<T1, T2>> GetResponse<T1, T2>(TRequest message, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

#### Parameters

`message` TRequest<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[Task\<Response\<T1, T2\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponse\<T1, T2\>(TRequest, RequestPipeConfiguratorCallback\<TRequest\>, CancellationToken, RequestTimeout)**

```csharp
public Task<Response<T1, T2>> GetResponse<T1, T2>(TRequest message, RequestPipeConfiguratorCallback<TRequest> callback, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

#### Parameters

`message` TRequest<br/>

`callback` [RequestPipeConfiguratorCallback\<TRequest\>](../../masstransit-abstractions/masstransit/requestpipeconfiguratorcallback-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[Task\<Response\<T1, T2\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponse\<T1, T2\>(Object, CancellationToken, RequestTimeout)**

```csharp
public Task<Response<T1, T2>> GetResponse<T1, T2>(object values, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[Task\<Response\<T1, T2\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponse\<T1, T2\>(Object, RequestPipeConfiguratorCallback\<TRequest\>, CancellationToken, RequestTimeout)**

```csharp
public Task<Response<T1, T2>> GetResponse<T1, T2>(object values, RequestPipeConfiguratorCallback<TRequest> callback, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`callback` [RequestPipeConfiguratorCallback\<TRequest\>](../../masstransit-abstractions/masstransit/requestpipeconfiguratorcallback-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[Task\<Response\<T1, T2\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponse\<T1, T2, T3\>(TRequest, CancellationToken, RequestTimeout)**

```csharp
public Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(TRequest message, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`T3`<br/>

#### Parameters

`message` TRequest<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[Task\<Response\<T1, T2, T3\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponse\<T1, T2, T3\>(TRequest, RequestPipeConfiguratorCallback\<TRequest\>, CancellationToken, RequestTimeout)**

```csharp
public Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(TRequest message, RequestPipeConfiguratorCallback<TRequest> callback, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`T3`<br/>

#### Parameters

`message` TRequest<br/>

`callback` [RequestPipeConfiguratorCallback\<TRequest\>](../../masstransit-abstractions/masstransit/requestpipeconfiguratorcallback-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[Task\<Response\<T1, T2, T3\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponse\<T1, T2, T3\>(Object, CancellationToken, RequestTimeout)**

```csharp
public Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(object values, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`T3`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[Task\<Response\<T1, T2, T3\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponse\<T1, T2, T3\>(Object, RequestPipeConfiguratorCallback\<TRequest\>, CancellationToken, RequestTimeout)**

```csharp
public Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(object values, RequestPipeConfiguratorCallback<TRequest> callback, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`T3`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`callback` [RequestPipeConfiguratorCallback\<TRequest\>](../../masstransit-abstractions/masstransit/requestpipeconfiguratorcallback-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[Task\<Response\<T1, T2, T3\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
