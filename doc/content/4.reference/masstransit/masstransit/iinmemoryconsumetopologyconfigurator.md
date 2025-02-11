---

title: IInMemoryConsumeTopologyConfigurator

---

# IInMemoryConsumeTopologyConfigurator

Namespace: MassTransit

```csharp
public interface IInMemoryConsumeTopologyConfigurator : IConsumeTopologyConfigurator, IConsumeTopology, IConsumeTopologyConfigurationObserverConnector, ISpecification, IInMemoryConsumeTopology
```

Implements [IConsumeTopologyConfigurator](../../masstransit-abstractions/masstransit/iconsumetopologyconfigurator), [IConsumeTopology](../../masstransit-abstractions/masstransit/iconsumetopology), [IConsumeTopologyConfigurationObserverConnector](../../masstransit-abstractions/masstransit-configuration/iconsumetopologyconfigurationobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IInMemoryConsumeTopology](../masstransit/iinmemoryconsumetopology)

## Methods

### **GetMessageTopology\<T\>()**

```csharp
IInMemoryMessageConsumeTopologyConfigurator<T> GetMessageTopology<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IInMemoryMessageConsumeTopologyConfigurator\<T\>](../masstransit/iinmemorymessageconsumetopologyconfigurator-1)<br/>

### **AddSpecification(IInMemoryConsumeTopologySpecification)**

```csharp
void AddSpecification(IInMemoryConsumeTopologySpecification specification)
```

#### Parameters

`specification` [IInMemoryConsumeTopologySpecification](../masstransit-inmemorytransport-configuration/iinmemoryconsumetopologyspecification)<br/>

### **Bind(String, ExchangeType, String)**

```csharp
void Bind(string exchangeName, ExchangeType exchangeType, string routingKey)
```

#### Parameters

`exchangeName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exchangeType` [ExchangeType](../masstransit-transports-fabric/exchangetype)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
