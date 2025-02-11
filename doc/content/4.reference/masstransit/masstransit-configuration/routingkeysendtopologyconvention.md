---

title: RoutingKeySendTopologyConvention

---

# RoutingKeySendTopologyConvention

Namespace: MassTransit.Configuration

```csharp
public class RoutingKeySendTopologyConvention : IRoutingKeySendTopologyConvention, ISendTopologyConvention, IMessageSendTopologyConvention
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingKeySendTopologyConvention](../masstransit-configuration/routingkeysendtopologyconvention)<br/>
Implements [IRoutingKeySendTopologyConvention](../masstransit-configuration/iroutingkeysendtopologyconvention), [ISendTopologyConvention](../../masstransit-abstractions/masstransit-configuration/isendtopologyconvention), [IMessageSendTopologyConvention](../../masstransit-abstractions/masstransit-configuration/imessagesendtopologyconvention)

## Constructors

### **RoutingKeySendTopologyConvention()**

```csharp
public RoutingKeySendTopologyConvention()
```

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
