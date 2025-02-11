---

title: IInMemoryMessageConsumeTopologyConfigurator

---

# IInMemoryMessageConsumeTopologyConfigurator

Namespace: MassTransit

```csharp
public interface IInMemoryMessageConsumeTopologyConfigurator : IMessageConsumeTopologyConfigurator, ISpecification
```

Implements [IMessageConsumeTopologyConfigurator](../../masstransit-abstractions/masstransit/imessageconsumetopologyconfigurator), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Methods

### **Apply(IMessageFabricConsumeTopologyBuilder)**

Apply the message topology to the builder

```csharp
void Apply(IMessageFabricConsumeTopologyBuilder builder)
```

#### Parameters

`builder` [IMessageFabricConsumeTopologyBuilder](../masstransit-configuration/imessagefabricconsumetopologybuilder)<br/>
