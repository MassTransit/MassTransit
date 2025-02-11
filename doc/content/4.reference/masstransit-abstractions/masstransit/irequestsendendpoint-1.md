---

title: IRequestSendEndpoint<T>

---

# IRequestSendEndpoint\<T\>

Namespace: MassTransit

```csharp
public interface IRequestSendEndpoint<T>
```

#### Type Parameters

`T`<br/>

## Methods

### **Send(Guid, Object, IPipe\<SendContext\<T\>\>, CancellationToken)**

```csharp
Task<T> Send(Guid requestId, object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Parameters

`requestId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Send(Guid, T, IPipe\<SendContext\<T\>\>, CancellationToken)**

```csharp
Task Send(Guid requestId, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Parameters

`requestId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`message` T<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
