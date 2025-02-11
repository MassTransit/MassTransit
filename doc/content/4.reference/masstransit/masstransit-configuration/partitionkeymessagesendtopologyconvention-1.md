---

title: PartitionKeyMessageSendTopologyConvention<TMessage>

---

# PartitionKeyMessageSendTopologyConvention\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class PartitionKeyMessageSendTopologyConvention<TMessage> : IPartitionKeyMessageSendTopologyConvention<TMessage>, IMessageSendTopologyConvention<TMessage>, IMessageSendTopologyConvention
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PartitionKeyMessageSendTopologyConvention\<TMessage\>](../masstransit-configuration/partitionkeymessagesendtopologyconvention-1)<br/>
Implements [IPartitionKeyMessageSendTopologyConvention\<TMessage\>](../masstransit-configuration/ipartitionkeymessagesendtopologyconvention-1), [IMessageSendTopologyConvention\<TMessage\>](../../masstransit-abstractions/masstransit-configuration/imessagesendtopologyconvention-1), [IMessageSendTopologyConvention](../../masstransit-abstractions/masstransit-configuration/imessagesendtopologyconvention)

## Constructors

### **PartitionKeyMessageSendTopologyConvention(IPartitionKeyFormatter)**

```csharp
public PartitionKeyMessageSendTopologyConvention(IPartitionKeyFormatter formatter)
```

#### Parameters

`formatter` [IPartitionKeyFormatter](../masstransit-transports/ipartitionkeyformatter)<br/>

## Methods

### **TryGetMessageSendTopology(IMessageSendTopology\<TMessage\>)**

```csharp
public bool TryGetMessageSendTopology(out IMessageSendTopology<TMessage> messageSendTopology)
```

#### Parameters

`messageSendTopology` [IMessageSendTopology\<TMessage\>](../../masstransit-abstractions/masstransit/imessagesendtopology-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

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

### **SetFormatter(IPartitionKeyFormatter)**

```csharp
public void SetFormatter(IPartitionKeyFormatter formatter)
```

#### Parameters

`formatter` [IPartitionKeyFormatter](../masstransit-transports/ipartitionkeyformatter)<br/>

### **SetFormatter(IMessagePartitionKeyFormatter\<TMessage\>)**

```csharp
public void SetFormatter(IMessagePartitionKeyFormatter<TMessage> formatter)
```

#### Parameters

`formatter` [IMessagePartitionKeyFormatter\<TMessage\>](../masstransit-transports/imessagepartitionkeyformatter-1)<br/>
