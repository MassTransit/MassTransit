---

title: ISetSerializerMessageSendTopologyConvention<TMessage>

---

# ISetSerializerMessageSendTopologyConvention\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public interface ISetSerializerMessageSendTopologyConvention<TMessage> : IMessageSendTopologyConvention<TMessage>, IMessageSendTopologyConvention
```

#### Type Parameters

`TMessage`<br/>

Implements [IMessageSendTopologyConvention\<TMessage\>](../../masstransit-abstractions/masstransit-configuration/imessagesendtopologyconvention-1), [IMessageSendTopologyConvention](../../masstransit-abstractions/masstransit-configuration/imessagesendtopologyconvention)

## Methods

### **SetSerializer(ContentType)**

```csharp
void SetSerializer(ContentType contentType)
```

#### Parameters

`contentType` ContentType<br/>
