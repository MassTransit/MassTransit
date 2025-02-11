---

title: IRoutingKeyMessageSendTopologyConvention<TMessage>

---

# IRoutingKeyMessageSendTopologyConvention\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public interface IRoutingKeyMessageSendTopologyConvention<TMessage> : IMessageSendTopologyConvention<TMessage>, IMessageSendTopologyConvention
```

#### Type Parameters

`TMessage`<br/>

Implements [IMessageSendTopologyConvention\<TMessage\>](../../masstransit-abstractions/masstransit-configuration/imessagesendtopologyconvention-1), [IMessageSendTopologyConvention](../../masstransit-abstractions/masstransit-configuration/imessagesendtopologyconvention)

## Methods

### **SetFormatter(IRoutingKeyFormatter)**

```csharp
void SetFormatter(IRoutingKeyFormatter formatter)
```

#### Parameters

`formatter` [IRoutingKeyFormatter](../masstransit-transports/iroutingkeyformatter)<br/>

### **SetFormatter(IMessageRoutingKeyFormatter\<TMessage\>)**

```csharp
void SetFormatter(IMessageRoutingKeyFormatter<TMessage> formatter)
```

#### Parameters

`formatter` [IMessageRoutingKeyFormatter\<TMessage\>](../masstransit-transports/imessageroutingkeyformatter-1)<br/>
