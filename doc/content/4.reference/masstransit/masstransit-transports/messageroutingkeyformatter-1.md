---

title: MessageRoutingKeyFormatter<TMessage>

---

# MessageRoutingKeyFormatter\<TMessage\>

Namespace: MassTransit.Transports

```csharp
public class MessageRoutingKeyFormatter<TMessage> : IMessageRoutingKeyFormatter<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageRoutingKeyFormatter\<TMessage\>](../masstransit-transports/messageroutingkeyformatter-1)<br/>
Implements [IMessageRoutingKeyFormatter\<TMessage\>](../masstransit-transports/imessageroutingkeyformatter-1)

## Constructors

### **MessageRoutingKeyFormatter(IRoutingKeyFormatter)**

```csharp
public MessageRoutingKeyFormatter(IRoutingKeyFormatter formatter)
```

#### Parameters

`formatter` [IRoutingKeyFormatter](../masstransit-transports/iroutingkeyformatter)<br/>

## Methods

### **FormatRoutingKey(SendContext\<TMessage\>)**

```csharp
public string FormatRoutingKey(SendContext<TMessage> context)
```

#### Parameters

`context` [SendContext\<TMessage\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
