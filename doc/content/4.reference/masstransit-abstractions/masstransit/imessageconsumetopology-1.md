---

title: IMessageConsumeTopology<TMessage>

---

# IMessageConsumeTopology\<TMessage\>

Namespace: MassTransit

The message-specific Consume topology, which may be configured or otherwise
 setup for use with the Consume specification.

```csharp
public interface IMessageConsumeTopology<TMessage>
```

#### Type Parameters

`TMessage`<br/>

## Methods

### **Apply(ITopologyPipeBuilder\<ConsumeContext\<TMessage\>\>)**

```csharp
void Apply(ITopologyPipeBuilder<ConsumeContext<TMessage>> builder)
```

#### Parameters

`builder` [ITopologyPipeBuilder\<ConsumeContext\<TMessage\>\>](../masstransit-configuration/itopologypipebuilder-1)<br/>
