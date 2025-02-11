---

title: MessagePartitionKeyFormatter<TMessage>

---

# MessagePartitionKeyFormatter\<TMessage\>

Namespace: MassTransit.Transports

```csharp
public class MessagePartitionKeyFormatter<TMessage> : IMessagePartitionKeyFormatter<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessagePartitionKeyFormatter\<TMessage\>](../masstransit-transports/messagepartitionkeyformatter-1)<br/>
Implements [IMessagePartitionKeyFormatter\<TMessage\>](../masstransit-transports/imessagepartitionkeyformatter-1)

## Constructors

### **MessagePartitionKeyFormatter(IPartitionKeyFormatter)**

```csharp
public MessagePartitionKeyFormatter(IPartitionKeyFormatter formatter)
```

#### Parameters

`formatter` [IPartitionKeyFormatter](../masstransit-transports/ipartitionkeyformatter)<br/>

## Methods

### **FormatPartitionKey(SendContext\<TMessage\>)**

```csharp
public string FormatPartitionKey(SendContext<TMessage> context)
```

#### Parameters

`context` [SendContext\<TMessage\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
