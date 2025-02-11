---

title: IMessageSendTopologyConvention<TMessage>

---

# IMessageSendTopologyConvention\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public interface IMessageSendTopologyConvention<TMessage> : IMessageSendTopologyConvention
```

#### Type Parameters

`TMessage`<br/>

Implements [IMessageSendTopologyConvention](../masstransit-configuration/imessagesendtopologyconvention)

## Methods

### **TryGetMessageSendTopology(IMessageSendTopology\<TMessage\>)**

```csharp
bool TryGetMessageSendTopology(out IMessageSendTopology<TMessage> messageSendTopology)
```

#### Parameters

`messageSendTopology` [IMessageSendTopology\<TMessage\>](../masstransit/imessagesendtopology-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
