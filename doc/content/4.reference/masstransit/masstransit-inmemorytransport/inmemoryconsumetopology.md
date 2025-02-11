---

title: InMemoryConsumeTopology

---

# InMemoryConsumeTopology

Namespace: MassTransit.InMemoryTransport

```csharp
public class InMemoryConsumeTopology : ConsumeTopology, IConsumeTopologyConfigurator, IConsumeTopology, IConsumeTopologyConfigurationObserverConnector, ISpecification, IConsumeTopologyConfigurationObserver, IInMemoryConsumeTopologyConfigurator, IInMemoryConsumeTopology
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumeTopology](../masstransit/consumetopology) → [InMemoryConsumeTopology](../masstransit-inmemorytransport/inmemoryconsumetopology)<br/>
Implements [IConsumeTopologyConfigurator](../../masstransit-abstractions/masstransit/iconsumetopologyconfigurator), [IConsumeTopology](../../masstransit-abstractions/masstransit/iconsumetopology), [IConsumeTopologyConfigurationObserverConnector](../../masstransit-abstractions/masstransit-configuration/iconsumetopologyconfigurationobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IConsumeTopologyConfigurationObserver](../../masstransit-abstractions/masstransit-configuration/iconsumetopologyconfigurationobserver), [IInMemoryConsumeTopologyConfigurator](../masstransit/iinmemoryconsumetopologyconfigurator), [IInMemoryConsumeTopology](../masstransit/iinmemoryconsumetopology)

## Constructors

### **InMemoryConsumeTopology(IMessageTopology, IInMemoryPublishTopologyConfigurator)**

```csharp
public InMemoryConsumeTopology(IMessageTopology messageTopology, IInMemoryPublishTopologyConfigurator publishTopology)
```

#### Parameters

`messageTopology` [IMessageTopology](../../masstransit-abstractions/masstransit/imessagetopology)<br/>

`publishTopology` [IInMemoryPublishTopologyConfigurator](../masstransit/iinmemorypublishtopologyconfigurator)<br/>

## Methods

### **AddSpecification(IInMemoryConsumeTopologySpecification)**

```csharp
public void AddSpecification(IInMemoryConsumeTopologySpecification specification)
```

#### Parameters

`specification` [IInMemoryConsumeTopologySpecification](../masstransit-inmemorytransport-configuration/iinmemoryconsumetopologyspecification)<br/>

### **Bind(String, ExchangeType, String)**

```csharp
public void Bind(string exchangeName, ExchangeType exchangeType, string routingKey)
```

#### Parameters

`exchangeName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exchangeType` [ExchangeType](../masstransit-transports-fabric/exchangetype)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Apply(IMessageFabricConsumeTopologyBuilder)**

```csharp
public void Apply(IMessageFabricConsumeTopologyBuilder builder)
```

#### Parameters

`builder` [IMessageFabricConsumeTopologyBuilder](../masstransit-configuration/imessagefabricconsumetopologybuilder)<br/>

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
