---

title: MessageDataSendTopologyConvention

---

# MessageDataSendTopologyConvention

Namespace: MassTransit.MessageData.Conventions

```csharp
public class MessageDataSendTopologyConvention : ISendTopologyConvention, IMessageSendTopologyConvention
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageDataSendTopologyConvention](../masstransit-messagedata-conventions/messagedatasendtopologyconvention)<br/>
Implements [ISendTopologyConvention](../../masstransit-abstractions/masstransit-configuration/isendtopologyconvention), [IMessageSendTopologyConvention](../../masstransit-abstractions/masstransit-configuration/imessagesendtopologyconvention)

## Constructors

### **MessageDataSendTopologyConvention(IMessageDataRepository)**

```csharp
public MessageDataSendTopologyConvention(IMessageDataRepository repository)
```

#### Parameters

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

## Methods

### **TryGetMessageSendTopologyConvention\<T\>(IMessageSendTopologyConvention\<T\>)**

```csharp
public bool TryGetMessageSendTopologyConvention<T>(out IMessageSendTopologyConvention<T> convention)
```

#### Type Parameters

`T`<br/>

#### Parameters

`convention` [IMessageSendTopologyConvention\<T\>](../../masstransit-abstractions/masstransit-configuration/imessagesendtopologyconvention-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
