---

title: ICorrelationIdMessageSendTopologyConvention<TMessage>

---

# ICorrelationIdMessageSendTopologyConvention\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public interface ICorrelationIdMessageSendTopologyConvention<TMessage> : IMessageSendTopologyConvention<TMessage>, IMessageSendTopologyConvention
```

#### Type Parameters

`TMessage`<br/>

Implements [IMessageSendTopologyConvention\<TMessage\>](../../masstransit-abstractions/masstransit-configuration/imessagesendtopologyconvention-1), [IMessageSendTopologyConvention](../../masstransit-abstractions/masstransit-configuration/imessagesendtopologyconvention)

## Methods

### **SetCorrelationId(IMessageCorrelationId\<TMessage\>)**

```csharp
void SetCorrelationId(IMessageCorrelationId<TMessage> messageCorrelationId)
```

#### Parameters

`messageCorrelationId` [IMessageCorrelationId\<TMessage\>](../masstransit/imessagecorrelationid-1)<br/>

### **TryGetMessageCorrelationId(IMessageCorrelationId\<TMessage\>)**

Tries to get the message correlation id

```csharp
bool TryGetMessageCorrelationId(out IMessageCorrelationId<TMessage> messageCorrelationId)
```

#### Parameters

`messageCorrelationId` [IMessageCorrelationId\<TMessage\>](../masstransit/imessagecorrelationid-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
