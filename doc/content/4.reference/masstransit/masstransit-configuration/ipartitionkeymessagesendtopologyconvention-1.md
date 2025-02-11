---

title: IPartitionKeyMessageSendTopologyConvention<TMessage>

---

# IPartitionKeyMessageSendTopologyConvention\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public interface IPartitionKeyMessageSendTopologyConvention<TMessage> : IMessageSendTopologyConvention<TMessage>, IMessageSendTopologyConvention
```

#### Type Parameters

`TMessage`<br/>

Implements [IMessageSendTopologyConvention\<TMessage\>](../../masstransit-abstractions/masstransit-configuration/imessagesendtopologyconvention-1), [IMessageSendTopologyConvention](../../masstransit-abstractions/masstransit-configuration/imessagesendtopologyconvention)

## Methods

### **SetFormatter(IPartitionKeyFormatter)**

```csharp
void SetFormatter(IPartitionKeyFormatter formatter)
```

#### Parameters

`formatter` [IPartitionKeyFormatter](../masstransit-transports/ipartitionkeyformatter)<br/>

### **SetFormatter(IMessagePartitionKeyFormatter\<TMessage\>)**

```csharp
void SetFormatter(IMessagePartitionKeyFormatter<TMessage> formatter)
```

#### Parameters

`formatter` [IMessagePartitionKeyFormatter\<TMessage\>](../masstransit-transports/imessagepartitionkeyformatter-1)<br/>
