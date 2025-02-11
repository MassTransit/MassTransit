---

title: RequestHandle<TRequest>

---

# RequestHandle\<TRequest\>

Namespace: MassTransit

A request handle manages the client-side request, and allows the request to be configured, response types added, etc. The handle
 should be disposed once it is no longer in-use, and the request has been completed (successfully, or otherwise).

```csharp
public interface RequestHandle<TRequest> : RequestHandle, IRequestPipeConfigurator, IDisposable, IRequestPipeConfigurator<TRequest>, IPipeConfigurator<SendContext<TRequest>>
```

#### Type Parameters

`TRequest`<br/>
The request type

Implements [RequestHandle](../masstransit/requesthandle), [IRequestPipeConfigurator](../masstransit/irequestpipeconfigurator), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable), [IRequestPipeConfigurator\<TRequest\>](../masstransit/irequestpipeconfigurator-1), [IPipeConfigurator\<SendContext\<TRequest\>\>](../masstransit/ipipeconfigurator-1)

## Properties

### **Message**

The request message that was/will be sent.

```csharp
public abstract Task<TRequest> Message { get; }
```

#### Property Value

[Task\<TRequest\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
