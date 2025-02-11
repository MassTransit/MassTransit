---

title: MessageDataMessageSendTopology<T>

---

# MessageDataMessageSendTopology\<T\>

Namespace: MassTransit.MessageData.Conventions

```csharp
public class MessageDataMessageSendTopology<T> : IMessageSendTopology<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageDataMessageSendTopology\<T\>](../masstransit-messagedata-conventions/messagedatamessagesendtopology-1)<br/>
Implements [IMessageSendTopology\<T\>](../../masstransit-abstractions/masstransit/imessagesendtopology-1)

## Constructors

### **MessageDataMessageSendTopology(IMessageInitializer\<T\>)**

```csharp
public MessageDataMessageSendTopology(IMessageInitializer<T> initializer)
```

#### Parameters

`initializer` [IMessageInitializer\<T\>](../../masstransit-abstractions/masstransit-initializers/imessageinitializer-1)<br/>

## Methods

### **Apply(ITopologyPipeBuilder\<SendContext\<T\>\>)**

```csharp
public void Apply(ITopologyPipeBuilder<SendContext<T>> builder)
```

#### Parameters

`builder` [ITopologyPipeBuilder\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/itopologypipebuilder-1)<br/>
