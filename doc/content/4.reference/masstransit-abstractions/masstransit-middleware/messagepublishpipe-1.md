---

title: MessagePublishPipe<TMessage>

---

# MessagePublishPipe\<TMessage\>

Namespace: MassTransit.Middleware

Converts an inbound context type to a pipe context type post-dispatch

```csharp
public class MessagePublishPipe<TMessage> : IMessagePublishPipe<TMessage>, IPipe<PublishContext<TMessage>>, IProbeSite
```

#### Type Parameters

`TMessage`<br/>
The subsequent pipe context type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessagePublishPipe\<TMessage\>](../masstransit-middleware/messagepublishpipe-1)<br/>
Implements [IMessagePublishPipe\<TMessage\>](../masstransit-middleware/imessagepublishpipe-1), [IPipe\<PublishContext\<TMessage\>\>](../masstransit/ipipe-1), [IProbeSite](../masstransit/iprobesite)

## Constructors

### **MessagePublishPipe(IPipe\<PublishContext\<TMessage\>\>)**

```csharp
public MessagePublishPipe(IPipe<PublishContext<TMessage>> outputPipe)
```

#### Parameters

`outputPipe` [IPipe\<PublishContext\<TMessage\>\>](../masstransit/ipipe-1)<br/>
