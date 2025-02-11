---

title: IMessageDataRepository

---

# IMessageDataRepository

Namespace: MassTransit

Storage of large message data that can be stored and retrieved separate of the message body.
 Implemented as a claim-check pattern when an identifier is stored in the message body which
 is used to retrieve the message data separately.

```csharp
public interface IMessageDataRepository
```

## Methods

### **Get(Uri, CancellationToken)**

Returns a stream to read the message data for the specified address.

```csharp
Task<Stream> Get(Uri address, CancellationToken cancellationToken)
```

#### Parameters

`address` Uri<br/>
The data address

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
A cancellation token for the request

#### Returns

[Task\<Stream\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Put(Stream, Nullable\<TimeSpan\>, CancellationToken)**

Puts message data into the repository

```csharp
Task<Uri> Put(Stream stream, Nullable<TimeSpan> timeToLive, CancellationToken cancellationToken)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>
The stream of data for the message

`timeToLive` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Uri\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
