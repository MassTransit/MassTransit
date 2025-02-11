---

title: MessageDataMessageConsumeTopologyConvention<TMessage>

---

# MessageDataMessageConsumeTopologyConvention\<TMessage\>

Namespace: MassTransit.MessageData.Conventions

```csharp
public class MessageDataMessageConsumeTopologyConvention<TMessage> : IMessageDataMessageConsumeTopologyConvention<TMessage>, IMessageConsumeTopologyConvention<TMessage>, IMessageConsumeTopologyConvention
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageDataMessageConsumeTopologyConvention\<TMessage\>](../masstransit-messagedata-conventions/messagedatamessageconsumetopologyconvention-1)<br/>
Implements [IMessageDataMessageConsumeTopologyConvention\<TMessage\>](../masstransit-messagedata-conventions/imessagedatamessageconsumetopologyconvention-1), [IMessageConsumeTopologyConvention\<TMessage\>](../../masstransit-abstractions/masstransit-configuration/imessageconsumetopologyconvention-1), [IMessageConsumeTopologyConvention](../../masstransit-abstractions/masstransit-configuration/imessageconsumetopologyconvention)

## Constructors

### **MessageDataMessageConsumeTopologyConvention(IMessageDataRepository)**

```csharp
public MessageDataMessageConsumeTopologyConvention(IMessageDataRepository repository)
```

#### Parameters

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

## Methods

### **TryGetMessageConsumeTopology(IMessageConsumeTopology\<TMessage\>)**

```csharp
public bool TryGetMessageConsumeTopology(out IMessageConsumeTopology<TMessage> messageConsumeTopology)
```

#### Parameters

`messageConsumeTopology` [IMessageConsumeTopology\<TMessage\>](../../masstransit-abstractions/masstransit/imessageconsumetopology-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
