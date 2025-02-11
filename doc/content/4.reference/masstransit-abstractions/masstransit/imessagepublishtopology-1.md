---

title: IMessagePublishTopology<TMessage>

---

# IMessagePublishTopology\<TMessage\>

Namespace: MassTransit

The message-specific publish topology, which may be configured or otherwise
 setup for use with the publish specification.

```csharp
public interface IMessagePublishTopology<TMessage> : IMessagePublishTopology
```

#### Type Parameters

`TMessage`<br/>

Implements [IMessagePublishTopology](../masstransit/imessagepublishtopology)

## Methods

### **Apply(ITopologyPipeBuilder\<PublishContext\<TMessage\>\>)**

```csharp
void Apply(ITopologyPipeBuilder<PublishContext<TMessage>> builder)
```

#### Parameters

`builder` [ITopologyPipeBuilder\<PublishContext\<TMessage\>\>](../masstransit-configuration/itopologypipebuilder-1)<br/>
