---

title: IInMemoryConsumeTopology

---

# IInMemoryConsumeTopology

Namespace: MassTransit

```csharp
public interface IInMemoryConsumeTopology : IConsumeTopology, IConsumeTopologyConfigurationObserverConnector
```

Implements [IConsumeTopology](../../masstransit-abstractions/masstransit/iconsumetopology), [IConsumeTopologyConfigurationObserverConnector](../../masstransit-abstractions/masstransit-configuration/iconsumetopologyconfigurationobserverconnector)

## Methods

### **GetMessageTopology\<T\>()**

```csharp
IInMemoryMessageConsumeTopology<T> GetMessageTopology<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IInMemoryMessageConsumeTopology\<T\>](../masstransit/iinmemorymessageconsumetopology-1)<br/>

### **Apply(IMessageFabricConsumeTopologyBuilder)**

Apply the entire topology to the builder

```csharp
void Apply(IMessageFabricConsumeTopologyBuilder builder)
```

#### Parameters

`builder` [IMessageFabricConsumeTopologyBuilder](../masstransit-configuration/imessagefabricconsumetopologybuilder)<br/>
