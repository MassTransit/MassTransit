---

title: EncryptedMessageDataRepository

---

# EncryptedMessageDataRepository

Namespace: MassTransit.MessageData

```csharp
public class EncryptedMessageDataRepository : IMessageDataRepository
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EncryptedMessageDataRepository](../masstransit-messagedata/encryptedmessagedatarepository)<br/>
Implements [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)

## Constructors

### **EncryptedMessageDataRepository(IMessageDataRepository, ICryptoStreamProvider)**

Provides encrypted stream support to ensure that message data is encrypted at rest.

```csharp
public EncryptedMessageDataRepository(IMessageDataRepository repository, ICryptoStreamProvider streamProvider)
```

#### Parameters

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>
The original message data repository where message data is stored.

`streamProvider` [ICryptoStreamProvider](../masstransit-serialization/icryptostreamprovider)<br/>
The encrypted stream provider

## Methods

### **Get(Uri, CancellationToken)**

```csharp
public Task<Stream> Get(Uri address, CancellationToken cancellationToken)
```

#### Parameters

`address` Uri<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Stream\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Put(Stream, Nullable\<TimeSpan\>, CancellationToken)**

```csharp
public Task<Uri> Put(Stream stream, Nullable<TimeSpan> timeToLive, CancellationToken cancellationToken)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>

`timeToLive` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Uri\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
