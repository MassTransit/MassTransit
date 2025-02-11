---

title: ExchangeBindingConsumeTopologySpecification

---

# ExchangeBindingConsumeTopologySpecification

Namespace: MassTransit.InMemoryTransport.Configuration

Used to bind an exchange to the consuming queue's exchange

```csharp
public class ExchangeBindingConsumeTopologySpecification : IInMemoryConsumeTopologySpecification, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExchangeBindingConsumeTopologySpecification](../masstransit-inmemorytransport-configuration/exchangebindingconsumetopologyspecification)<br/>
Implements [IInMemoryConsumeTopologySpecification](../masstransit-inmemorytransport-configuration/iinmemoryconsumetopologyspecification), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **ExchangeBindingConsumeTopologySpecification(String, ExchangeType, String)**

```csharp
public ExchangeBindingConsumeTopologySpecification(string exchange, ExchangeType exchangeType, string routingKey)
```

#### Parameters

`exchange` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exchangeType` [ExchangeType](../masstransit-transports-fabric/exchangetype)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Apply(IMessageFabricConsumeTopologyBuilder)**

```csharp
public void Apply(IMessageFabricConsumeTopologyBuilder builder)
```

#### Parameters

`builder` [IMessageFabricConsumeTopologyBuilder](../masstransit-configuration/imessagefabricconsumetopologybuilder)<br/>
