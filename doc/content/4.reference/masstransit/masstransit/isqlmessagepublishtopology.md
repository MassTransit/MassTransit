---

title: ISqlMessagePublishTopology

---

# ISqlMessagePublishTopology

Namespace: MassTransit

```csharp
public interface ISqlMessagePublishTopology
```

## Methods

### **Apply(IPublishEndpointBrokerTopologyBuilder)**

Apply the message topology to the builder, including any implemented types

```csharp
void Apply(IPublishEndpointBrokerTopologyBuilder builder)
```

#### Parameters

`builder` [IPublishEndpointBrokerTopologyBuilder](../masstransit-sqltransport-topology/ipublishendpointbrokertopologybuilder)<br/>
The topology builder
