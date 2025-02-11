---

title: MediatorRequestExtensions

---

# MediatorRequestExtensions

Namespace: MassTransit

```csharp
public static class MediatorRequestExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MediatorRequestExtensions](../masstransit/mediatorrequestextensions)

## Methods

### **SendRequest\<T\>(IMediator, Request\<T\>, CancellationToken, RequestTimeout)**

Sends a request, with the specified response type, and awaits the response.

```csharp
public static Task<T> SendRequest<T>(IMediator mediator, Request<T> request, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>
The response type

#### Parameters

`mediator` [IMediator](../masstransit-mediator/imediator)<br/>

`request` [Request\<T\>](../masstransit-mediator/request-1)<br/>
The request message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The response object
