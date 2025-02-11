---

title: HandlerConnectHandle<T>

---

# HandlerConnectHandle\<T\>

Namespace: MassTransit.Clients

```csharp
public interface HandlerConnectHandle<T> : HandlerConnectHandle, ConnectHandle, IDisposable
```

#### Type Parameters

`T`<br/>

Implements [HandlerConnectHandle](../masstransit-clients/handlerconnecthandle), [ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **Task**

```csharp
public abstract Task<Response<T>> Task { get; }
```

#### Property Value

[Task\<Response\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
