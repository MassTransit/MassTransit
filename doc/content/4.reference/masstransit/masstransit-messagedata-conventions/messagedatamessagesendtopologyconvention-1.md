---

title: MessageDataMessageSendTopologyConvention<TMessage>

---

# MessageDataMessageSendTopologyConvention\<TMessage\>

Namespace: MassTransit.MessageData.Conventions

```csharp
public class MessageDataMessageSendTopologyConvention<TMessage> : IMessageDataMessageSendTopologyConvention<TMessage>, IMessageSendTopologyConvention<TMessage>, IMessageSendTopologyConvention
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageDataMessageSendTopologyConvention\<TMessage\>](../masstransit-messagedata-conventions/messagedatamessagesendtopologyconvention-1)<br/>
Implements [IMessageDataMessageSendTopologyConvention\<TMessage\>](../masstransit-messagedata-conventions/imessagedatamessagesendtopologyconvention-1), [IMessageSendTopologyConvention\<TMessage\>](../../masstransit-abstractions/masstransit-configuration/imessagesendtopologyconvention-1), [IMessageSendTopologyConvention](../../masstransit-abstractions/masstransit-configuration/imessagesendtopologyconvention)

## Constructors

### **MessageDataMessageSendTopologyConvention(IMessageDataRepository)**

```csharp
public MessageDataMessageSendTopologyConvention(IMessageDataRepository repository)
```

#### Parameters

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>
