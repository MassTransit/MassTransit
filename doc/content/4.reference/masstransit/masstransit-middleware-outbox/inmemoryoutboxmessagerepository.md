---

title: InMemoryOutboxMessageRepository

---

# InMemoryOutboxMessageRepository

Namespace: MassTransit.Middleware.Outbox

```csharp
public class InMemoryOutboxMessageRepository
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryOutboxMessageRepository](../masstransit-middleware-outbox/inmemoryoutboxmessagerepository)

## Constructors

### **InMemoryOutboxMessageRepository()**

```csharp
public InMemoryOutboxMessageRepository()
```

## Methods

### **MarkInUse(CancellationToken)**

```csharp
public Task MarkInUse(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Lock(Guid, Guid, CancellationToken)**

```csharp
public Task<InMemoryInboxMessage> Lock(Guid messageId, Guid consumerId, CancellationToken cancellationToken)
```

#### Parameters

`messageId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`consumerId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<InMemoryInboxMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Release()**

```csharp
public void Release()
```
