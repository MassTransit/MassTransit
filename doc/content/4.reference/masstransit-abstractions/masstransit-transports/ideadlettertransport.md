---

title: IDeadLetterTransport

---

# IDeadLetterTransport

Namespace: MassTransit.Transports

If present, can be used to move the [ReceiveContext](../masstransit/receivecontext) to the dead letter queue

```csharp
public interface IDeadLetterTransport
```

## Methods

### **Send(ReceiveContext, String)**

Writes the message to the dead letter queue, adding the reason as a transport header

```csharp
Task Send(ReceiveContext context, string reason)
```

#### Parameters

`context` [ReceiveContext](../masstransit/receivecontext)<br/>

`reason` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
