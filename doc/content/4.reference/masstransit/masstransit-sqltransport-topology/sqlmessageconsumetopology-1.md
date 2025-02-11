---

title: SqlMessageConsumeTopology<TMessage>

---

# SqlMessageConsumeTopology\<TMessage\>

Namespace: MassTransit.SqlTransport.Topology

```csharp
public class SqlMessageConsumeTopology<TMessage> : MessageConsumeTopology<TMessage>, IMessageConsumeTopologyConfigurator<TMessage>, IMessageConsumeTopologyConfigurator, ISpecification, IMessageConsumeTopology<TMessage>, ISqlMessageConsumeTopologyConfigurator<TMessage>, ISqlMessageConsumeTopology<TMessage>, IDbMessageConsumeTopologyConfigurator
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageConsumeTopology\<TMessage\>](../masstransit/messageconsumetopology-1) → [SqlMessageConsumeTopology\<TMessage\>](../masstransit-sqltransport-topology/sqlmessageconsumetopology-1)<br/>
Implements [IMessageConsumeTopologyConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/imessageconsumetopologyconfigurator-1), [IMessageConsumeTopologyConfigurator](../../masstransit-abstractions/masstransit/imessageconsumetopologyconfigurator), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IMessageConsumeTopology\<TMessage\>](../../masstransit-abstractions/masstransit/imessageconsumetopology-1), [ISqlMessageConsumeTopologyConfigurator\<TMessage\>](../masstransit/isqlmessageconsumetopologyconfigurator-1), [ISqlMessageConsumeTopology\<TMessage\>](../masstransit/isqlmessageconsumetopology-1), [IDbMessageConsumeTopologyConfigurator](../masstransit/idbmessageconsumetopologyconfigurator)

## Properties

### **ConfigureConsumeTopology**

```csharp
public bool ConfigureConsumeTopology { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **SqlMessageConsumeTopology(ISqlMessagePublishTopology\<TMessage\>)**

```csharp
public SqlMessageConsumeTopology(ISqlMessagePublishTopology<TMessage> publishTopology)
```

#### Parameters

`publishTopology` [ISqlMessagePublishTopology\<TMessage\>](../masstransit/isqlmessagepublishtopology-1)<br/>

## Methods

### **Apply(IReceiveEndpointBrokerTopologyBuilder)**

```csharp
public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
```

#### Parameters

`builder` [IReceiveEndpointBrokerTopologyBuilder](../masstransit-sqltransport-topology/ireceiveendpointbrokertopologybuilder)<br/>

### **Subscribe(Action\<ISqlTopicSubscriptionConfigurator\>)**

```csharp
public void Subscribe(Action<ISqlTopicSubscriptionConfigurator> configure)
```

#### Parameters

`configure` [Action\<ISqlTopicSubscriptionConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
