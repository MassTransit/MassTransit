---

title: InMemoryMessageConsumeTopology<TMessage>

---

# InMemoryMessageConsumeTopology\<TMessage\>

Namespace: MassTransit.InMemoryTransport

```csharp
public class InMemoryMessageConsumeTopology<TMessage> : MessageConsumeTopology<TMessage>, IMessageConsumeTopologyConfigurator<TMessage>, IMessageConsumeTopologyConfigurator, ISpecification, IMessageConsumeTopology<TMessage>, IInMemoryMessageConsumeTopologyConfigurator<TMessage>, IInMemoryMessageConsumeTopology<TMessage>, IInMemoryMessageConsumeTopologyConfigurator
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageConsumeTopology\<TMessage\>](../masstransit/messageconsumetopology-1) → [InMemoryMessageConsumeTopology\<TMessage\>](../masstransit-inmemorytransport/inmemorymessageconsumetopology-1)<br/>
Implements [IMessageConsumeTopologyConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/imessageconsumetopologyconfigurator-1), [IMessageConsumeTopologyConfigurator](../../masstransit-abstractions/masstransit/imessageconsumetopologyconfigurator), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IMessageConsumeTopology\<TMessage\>](../../masstransit-abstractions/masstransit/imessageconsumetopology-1), [IInMemoryMessageConsumeTopologyConfigurator\<TMessage\>](../masstransit/iinmemorymessageconsumetopologyconfigurator-1), [IInMemoryMessageConsumeTopology\<TMessage\>](../masstransit/iinmemorymessageconsumetopology-1), [IInMemoryMessageConsumeTopologyConfigurator](../masstransit/iinmemorymessageconsumetopologyconfigurator)

## Properties

### **ConfigureConsumeTopology**

```csharp
public bool ConfigureConsumeTopology { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **InMemoryMessageConsumeTopology(IMessageTopology\<TMessage\>, IInMemoryPublishTopologyConfigurator)**

```csharp
public InMemoryMessageConsumeTopology(IMessageTopology<TMessage> messageTopology, IInMemoryPublishTopologyConfigurator publishTopology)
```

#### Parameters

`messageTopology` [IMessageTopology\<TMessage\>](../../masstransit-abstractions/masstransit/imessagetopology-1)<br/>

`publishTopology` [IInMemoryPublishTopologyConfigurator](../masstransit/iinmemorypublishtopologyconfigurator)<br/>

## Methods

### **Apply(IMessageFabricConsumeTopologyBuilder)**

```csharp
public void Apply(IMessageFabricConsumeTopologyBuilder builder)
```

#### Parameters

`builder` [IMessageFabricConsumeTopologyBuilder](../masstransit-configuration/imessagefabricconsumetopologybuilder)<br/>

### **Bind(Nullable\<ExchangeType\>, String)**

```csharp
public void Bind(Nullable<ExchangeType> exchangeType, string routingKey)
```

#### Parameters

`exchangeType` [Nullable\<ExchangeType\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
