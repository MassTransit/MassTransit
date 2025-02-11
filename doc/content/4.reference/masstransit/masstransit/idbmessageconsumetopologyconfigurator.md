---

title: IDbMessageConsumeTopologyConfigurator

---

# IDbMessageConsumeTopologyConfigurator

Namespace: MassTransit

```csharp
public interface IDbMessageConsumeTopologyConfigurator : IMessageConsumeTopologyConfigurator, ISpecification
```

Implements [IMessageConsumeTopologyConfigurator](../../masstransit-abstractions/masstransit/imessageconsumetopologyconfigurator), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Methods

### **Apply(IReceiveEndpointBrokerTopologyBuilder)**

Apply the message topology to the builder

```csharp
void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
```

#### Parameters

`builder` [IReceiveEndpointBrokerTopologyBuilder](../masstransit-sqltransport-topology/ireceiveendpointbrokertopologybuilder)<br/>
