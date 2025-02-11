---

title: SetPartitionKeyMessageSendTopology<TMessage>

---

# SetPartitionKeyMessageSendTopology\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class SetPartitionKeyMessageSendTopology<TMessage> : IMessageSendTopology<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SetPartitionKeyMessageSendTopology\<TMessage\>](../masstransit-configuration/setpartitionkeymessagesendtopology-1)<br/>
Implements [IMessageSendTopology\<TMessage\>](../../masstransit-abstractions/masstransit/imessagesendtopology-1)

## Constructors

### **SetPartitionKeyMessageSendTopology(IMessagePartitionKeyFormatter\<TMessage\>)**

```csharp
public SetPartitionKeyMessageSendTopology(IMessagePartitionKeyFormatter<TMessage> partitionKeyFormatter)
```

#### Parameters

`partitionKeyFormatter` [IMessagePartitionKeyFormatter\<TMessage\>](../masstransit-transports/imessagepartitionkeyformatter-1)<br/>

## Methods

### **Apply(ITopologyPipeBuilder\<SendContext\<TMessage\>\>)**

```csharp
public void Apply(ITopologyPipeBuilder<SendContext<TMessage>> builder)
```

#### Parameters

`builder` [ITopologyPipeBuilder\<SendContext\<TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/itopologypipebuilder-1)<br/>
