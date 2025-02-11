---

title: ISqlConsumeTopology

---

# ISqlConsumeTopology

Namespace: MassTransit

```csharp
public interface ISqlConsumeTopology : IConsumeTopology, IConsumeTopologyConfigurationObserverConnector
```

Implements [IConsumeTopology](../../masstransit-abstractions/masstransit/iconsumetopology), [IConsumeTopologyConfigurationObserverConnector](../../masstransit-abstractions/masstransit-configuration/iconsumetopologyconfigurationobserverconnector)

## Methods

### **GetMessageTopology\<T\>()**

```csharp
ISqlMessageConsumeTopology<T> GetMessageTopology<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[ISqlMessageConsumeTopology\<T\>](../masstransit/isqlmessageconsumetopology-1)<br/>

### **Apply(IReceiveEndpointBrokerTopologyBuilder)**

Apply the entire topology to the builder

```csharp
void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
```

#### Parameters

`builder` [IReceiveEndpointBrokerTopologyBuilder](../masstransit-sqltransport-topology/ireceiveendpointbrokertopologybuilder)<br/>
