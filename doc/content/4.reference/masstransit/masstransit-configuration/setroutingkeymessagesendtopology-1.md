---

title: SetRoutingKeyMessageSendTopology<TMessage>

---

# SetRoutingKeyMessageSendTopology\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class SetRoutingKeyMessageSendTopology<TMessage> : IMessageSendTopology<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SetRoutingKeyMessageSendTopology\<TMessage\>](../masstransit-configuration/setroutingkeymessagesendtopology-1)<br/>
Implements [IMessageSendTopology\<TMessage\>](../../masstransit-abstractions/masstransit/imessagesendtopology-1)

## Constructors

### **SetRoutingKeyMessageSendTopology(IMessageRoutingKeyFormatter\<TMessage\>)**

```csharp
public SetRoutingKeyMessageSendTopology(IMessageRoutingKeyFormatter<TMessage> routingKeyFormatter)
```

#### Parameters

`routingKeyFormatter` [IMessageRoutingKeyFormatter\<TMessage\>](../masstransit-transports/imessageroutingkeyformatter-1)<br/>

## Methods

### **Apply(ITopologyPipeBuilder\<SendContext\<TMessage\>\>)**

```csharp
public void Apply(ITopologyPipeBuilder<SendContext<TMessage>> builder)
```

#### Parameters

`builder` [ITopologyPipeBuilder\<SendContext\<TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/itopologypipebuilder-1)<br/>
