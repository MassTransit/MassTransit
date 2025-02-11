---

title: ResponseHandlerConnectHandle<TResponse>

---

# ResponseHandlerConnectHandle\<TResponse\>

Namespace: MassTransit.Clients

A connection to a request which handles a result, and completes the Task when it's received

```csharp
public class ResponseHandlerConnectHandle<TResponse> : HandlerConnectHandle<TResponse>, HandlerConnectHandle, ConnectHandle, IDisposable
```

#### Type Parameters

`TResponse`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ResponseHandlerConnectHandle\<TResponse\>](../masstransit-clients/responsehandlerconnecthandle-1)<br/>
Implements [HandlerConnectHandle\<TResponse\>](../masstransit-clients/handlerconnecthandle-1), [HandlerConnectHandle](../masstransit-clients/handlerconnecthandle), [ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **Task**

```csharp
public Task<Response<TResponse>> Task { get; }
```

#### Property Value

[Task\<Response\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Constructors

### **ResponseHandlerConnectHandle(ConnectHandle, TaskCompletionSource\<ConsumeContext\<TResponse\>\>, Task)**

```csharp
public ResponseHandlerConnectHandle(ConnectHandle handle, TaskCompletionSource<ConsumeContext<TResponse>> completed, Task requestTask)
```

#### Parameters

`handle` [ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

`completed` [TaskCompletionSource\<ConsumeContext\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcompletionsource-1)<br/>

`requestTask` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

## Methods

### **Dispose()**

```csharp
public void Dispose()
```

### **Disconnect()**

```csharp
public void Disconnect()
```

### **TrySetException(Exception)**

```csharp
public void TrySetException(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **TrySetCanceled(CancellationToken)**

```csharp
public void TrySetCanceled(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
