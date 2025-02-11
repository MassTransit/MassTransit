---

title: ScopedRequestSendEndpoint<TRequest>

---

# ScopedRequestSendEndpoint\<TRequest\>

Namespace: MassTransit.DependencyInjection

```csharp
public class ScopedRequestSendEndpoint<TRequest> : IRequestSendEndpoint<TRequest>
```

#### Type Parameters

`TRequest`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopedRequestSendEndpoint\<TRequest\>](../masstransit-dependencyinjection/scopedrequestsendendpoint-1)<br/>
Implements [IRequestSendEndpoint\<TRequest\>](../../masstransit-abstractions/masstransit/irequestsendendpoint-1)

## Constructors

### **ScopedRequestSendEndpoint(IRequestSendEndpoint\<TRequest\>, IServiceProvider)**

```csharp
public ScopedRequestSendEndpoint(IRequestSendEndpoint<TRequest> endpoint, IServiceProvider serviceProvider)
```

#### Parameters

`endpoint` [IRequestSendEndpoint\<TRequest\>](../../masstransit-abstractions/masstransit/irequestsendendpoint-1)<br/>

`serviceProvider` IServiceProvider<br/>

## Methods

### **Send(Guid, Object, IPipe\<SendContext\<TRequest\>\>, CancellationToken)**

```csharp
public Task<TRequest> Send(Guid requestId, object values, IPipe<SendContext<TRequest>> pipe, CancellationToken cancellationToken)
```

#### Parameters

`requestId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\<TRequest\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<TRequest\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Send(Guid, TRequest, IPipe\<SendContext\<TRequest\>\>, CancellationToken)**

```csharp
public Task Send(Guid requestId, TRequest message, IPipe<SendContext<TRequest>> pipe, CancellationToken cancellationToken)
```

#### Parameters

`requestId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`message` TRequest<br/>

`pipe` [IPipe\<SendContext\<TRequest\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
