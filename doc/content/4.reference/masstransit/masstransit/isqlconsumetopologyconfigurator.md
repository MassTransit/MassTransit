---

title: ISqlConsumeTopologyConfigurator

---

# ISqlConsumeTopologyConfigurator

Namespace: MassTransit

```csharp
public interface ISqlConsumeTopologyConfigurator : IConsumeTopologyConfigurator, IConsumeTopology, IConsumeTopologyConfigurationObserverConnector, ISpecification, ISqlConsumeTopology
```

Implements [IConsumeTopologyConfigurator](../../masstransit-abstractions/masstransit/iconsumetopologyconfigurator), [IConsumeTopology](../../masstransit-abstractions/masstransit/iconsumetopology), [IConsumeTopologyConfigurationObserverConnector](../../masstransit-abstractions/masstransit-configuration/iconsumetopologyconfigurationobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [ISqlConsumeTopology](../masstransit/isqlconsumetopology)

## Methods

### **GetMessageTopology\<T\>()**

```csharp
ISqlMessageConsumeTopologyConfigurator<T> GetMessageTopology<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[ISqlMessageConsumeTopologyConfigurator\<T\>](../masstransit/isqlmessageconsumetopologyconfigurator-1)<br/>

### **AddSpecification(ISqlConsumeTopologySpecification)**

```csharp
void AddSpecification(ISqlConsumeTopologySpecification specification)
```

#### Parameters

`specification` [ISqlConsumeTopologySpecification](../masstransit-sqltransport-configuration/isqlconsumetopologyspecification)<br/>

### **Subscribe(String, Action\<ISqlTopicSubscriptionConfigurator\>)**

Bind an exchange, using the configurator

```csharp
void Subscribe(string topicName, Action<ISqlTopicSubscriptionConfigurator> configure)
```

#### Parameters

`topicName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configure` [Action\<ISqlTopicSubscriptionConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
