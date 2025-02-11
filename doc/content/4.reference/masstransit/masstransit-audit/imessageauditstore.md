---

title: IMessageAuditStore

---

# IMessageAuditStore

Namespace: MassTransit.Audit

Used to store message audits that are observed

```csharp
public interface IMessageAuditStore
```

## Methods

### **StoreMessage\<T\>(T, MessageAuditMetadata)**

Store the message audit, with associated metadata

```csharp
Task StoreMessage<T>(T message, MessageAuditMetadata metadata)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`message` T<br/>
The message itself

`metadata` [MessageAuditMetadata](../masstransit-audit/messageauditmetadata)<br/>
The message metadata

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
