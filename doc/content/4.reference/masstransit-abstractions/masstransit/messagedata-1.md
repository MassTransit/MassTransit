---

title: MessageData<T>

---

# MessageData\<T\>

Namespace: MassTransit

MessageData is used when a property size may be larger than what should be sent via the message
 transport. This would includes attachments such as images, documents, videos, etc. Using MessageData,
 it is possible to include large properties without sending them in the actual message. The claim check
 pattern is the common reference.

```csharp
public interface MessageData<T> : IMessageData
```

#### Type Parameters

`T`<br/>
The type used to access the message data, valid types include stream, string, and byte[].

Implements [IMessageData](../masstransit/imessagedata)

## Properties

### **Value**

The property value, which may be loaded asynchronously from the message data repository.

```csharp
public abstract Task<T> Value { get; }
```

#### Property Value

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
