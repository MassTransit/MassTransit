---

title: CorrelationIdMessageSendTopologyConvention<TMessage>

---

# CorrelationIdMessageSendTopologyConvention\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class CorrelationIdMessageSendTopologyConvention<TMessage> : ICorrelationIdMessageSendTopologyConvention<TMessage>, IMessageSendTopologyConvention<TMessage>, IMessageSendTopologyConvention
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CorrelationIdMessageSendTopologyConvention\<TMessage\>](../masstransit-configuration/correlationidmessagesendtopologyconvention-1)<br/>
Implements [ICorrelationIdMessageSendTopologyConvention\<TMessage\>](../masstransit-configuration/icorrelationidmessagesendtopologyconvention-1), [IMessageSendTopologyConvention\<TMessage\>](../../masstransit-abstractions/masstransit-configuration/imessagesendtopologyconvention-1), [IMessageSendTopologyConvention](../../masstransit-abstractions/masstransit-configuration/imessagesendtopologyconvention)

## Constructors

### **CorrelationIdMessageSendTopologyConvention()**

```csharp
public CorrelationIdMessageSendTopologyConvention()
```

## Methods

### **SetCorrelationId(IMessageCorrelationId\<TMessage\>)**

```csharp
public void SetCorrelationId(IMessageCorrelationId<TMessage> messageCorrelationId)
```

#### Parameters

`messageCorrelationId` [IMessageCorrelationId\<TMessage\>](../masstransit/imessagecorrelationid-1)<br/>

### **TryGetMessageCorrelationId(IMessageCorrelationId\<TMessage\>)**

```csharp
public bool TryGetMessageCorrelationId(out IMessageCorrelationId<TMessage> messageCorrelationId)
```

#### Parameters

`messageCorrelationId` [IMessageCorrelationId\<TMessage\>](../masstransit/imessagecorrelationid-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
