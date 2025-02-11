---

title: MessageSendPipe<TOutput>

---

# MessageSendPipe\<TOutput\>

Namespace: MassTransit.Middleware

Converts an inbound context type to a pipe context type post-dispatch

```csharp
public class MessageSendPipe<TOutput> : IMessageSendPipe<TOutput>, IPipe<SendContext<TOutput>>, IProbeSite
```

#### Type Parameters

`TOutput`<br/>
The subsequent pipe context type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageSendPipe\<TOutput\>](../masstransit-middleware/messagesendpipe-1)<br/>
Implements [IMessageSendPipe\<TOutput\>](../masstransit-middleware/imessagesendpipe-1), [IPipe\<SendContext\<TOutput\>\>](../masstransit/ipipe-1), [IProbeSite](../masstransit/iprobesite)

## Constructors

### **MessageSendPipe(IPipe\<SendContext\<TOutput\>\>)**

```csharp
public MessageSendPipe(IPipe<SendContext<TOutput>> outputPipe)
```

#### Parameters

`outputPipe` [IPipe\<SendContext\<TOutput\>\>](../masstransit/ipipe-1)<br/>
