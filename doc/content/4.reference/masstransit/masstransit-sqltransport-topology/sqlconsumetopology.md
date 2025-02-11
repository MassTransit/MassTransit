---

title: SqlConsumeTopology

---

# SqlConsumeTopology

Namespace: MassTransit.SqlTransport.Topology

```csharp
public class SqlConsumeTopology : ConsumeTopology, IConsumeTopologyConfigurator, IConsumeTopology, IConsumeTopologyConfigurationObserverConnector, ISpecification, IConsumeTopologyConfigurationObserver, ISqlConsumeTopologyConfigurator, ISqlConsumeTopology
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumeTopology](../masstransit/consumetopology) → [SqlConsumeTopology](../masstransit-sqltransport-topology/sqlconsumetopology)<br/>
Implements [IConsumeTopologyConfigurator](../../masstransit-abstractions/masstransit/iconsumetopologyconfigurator), [IConsumeTopology](../../masstransit-abstractions/masstransit/iconsumetopology), [IConsumeTopologyConfigurationObserverConnector](../../masstransit-abstractions/masstransit-configuration/iconsumetopologyconfigurationobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IConsumeTopologyConfigurationObserver](../../masstransit-abstractions/masstransit-configuration/iconsumetopologyconfigurationobserver), [ISqlConsumeTopologyConfigurator](../masstransit/isqlconsumetopologyconfigurator), [ISqlConsumeTopology](../masstransit/isqlconsumetopology)

## Constructors

### **SqlConsumeTopology(ISqlPublishTopology)**

```csharp
public SqlConsumeTopology(ISqlPublishTopology publishTopology)
```

#### Parameters

`publishTopology` [ISqlPublishTopology](../masstransit/isqlpublishtopology)<br/>

## Methods

### **AddSpecification(ISqlConsumeTopologySpecification)**

```csharp
public void AddSpecification(ISqlConsumeTopologySpecification specification)
```

#### Parameters

`specification` [ISqlConsumeTopologySpecification](../masstransit-sqltransport-configuration/isqlconsumetopologyspecification)<br/>

### **Apply(IReceiveEndpointBrokerTopologyBuilder)**

```csharp
public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
```

#### Parameters

`builder` [IReceiveEndpointBrokerTopologyBuilder](../masstransit-sqltransport-topology/ireceiveendpointbrokertopologybuilder)<br/>

### **Subscribe(String, Action\<ISqlTopicSubscriptionConfigurator\>)**

```csharp
public void Subscribe(string topicName, Action<ISqlTopicSubscriptionConfigurator> configure)
```

#### Parameters

`topicName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configure` [Action\<ISqlTopicSubscriptionConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **CreateMessageTopology\<T\>()**

```csharp
protected IMessageConsumeTopologyConfigurator CreateMessageTopology<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IMessageConsumeTopologyConfigurator](../../masstransit-abstractions/masstransit/imessageconsumetopologyconfigurator)<br/>
