---

title: RoutingKeyMessageSendTopologyConvention<TMessage>

---

# RoutingKeyMessageSendTopologyConvention\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class RoutingKeyMessageSendTopologyConvention<TMessage> : IRoutingKeyMessageSendTopologyConvention<TMessage>, IMessageSendTopologyConvention<TMessage>, IMessageSendTopologyConvention
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingKeyMessageSendTopologyConvention\<TMessage\>](../masstransit-configuration/routingkeymessagesendtopologyconvention-1)<br/>
Implements [IRoutingKeyMessageSendTopologyConvention\<TMessage\>](../masstransit-configuration/iroutingkeymessagesendtopologyconvention-1), [IMessageSendTopologyConvention\<TMessage\>](../../masstransit-abstractions/masstransit-configuration/imessagesendtopologyconvention-1), [IMessageSendTopologyConvention](../../masstransit-abstractions/masstransit-configuration/imessagesendtopologyconvention)

## Constructors

### **RoutingKeyMessageSendTopologyConvention(IRoutingKeyFormatter)**

```csharp
public RoutingKeyMessageSendTopologyConvention(IRoutingKeyFormatter formatter)
```

#### Parameters

`formatter` [IRoutingKeyFormatter](../masstransit-transports/iroutingkeyformatter)<br/>

## Methods

### **SetFormatter(IRoutingKeyFormatter)**

```csharp
public void SetFormatter(IRoutingKeyFormatter formatter)
```

#### Parameters

`formatter` [IRoutingKeyFormatter](../masstransit-transports/iroutingkeyformatter)<br/>

### **SetFormatter(IMessageRoutingKeyFormatter\<TMessage\>)**

```csharp
public void SetFormatter(IMessageRoutingKeyFormatter<TMessage> formatter)
```

#### Parameters

`formatter` [IMessageRoutingKeyFormatter\<TMessage\>](../masstransit-transports/imessageroutingkeyformatter-1)<br/>
