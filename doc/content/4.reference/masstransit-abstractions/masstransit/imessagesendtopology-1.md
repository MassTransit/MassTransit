---

title: IMessageSendTopology<TMessage>

---

# IMessageSendTopology\<TMessage\>

Namespace: MassTransit

The message-specific send topology, which may be configured or otherwise
 setup for use with the send specification.

```csharp
public interface IMessageSendTopology<TMessage>
```

#### Type Parameters

`TMessage`<br/>

## Methods

### **Apply(ITopologyPipeBuilder\<SendContext\<TMessage\>\>)**

```csharp
void Apply(ITopologyPipeBuilder<SendContext<TMessage>> builder)
```

#### Parameters

`builder` [ITopologyPipeBuilder\<SendContext\<TMessage\>\>](../masstransit-configuration/itopologypipebuilder-1)<br/>
