---

title: InMemoryMessagePublishTopology<TMessage>

---

# InMemoryMessagePublishTopology\<TMessage\>

Namespace: MassTransit.InMemoryTransport

```csharp
public class InMemoryMessagePublishTopology<TMessage> : MessagePublishTopology<TMessage>, IMessagePublishTopologyConfigurator<TMessage>, IMessagePublishTopologyConfigurator, IMessagePublishTopology, ISpecification, IMessagePublishTopology<TMessage>, IInMemoryMessagePublishTopologyConfigurator<TMessage>, IInMemoryMessagePublishTopology<TMessage>, IInMemoryMessagePublishTopology, IInMemoryMessagePublishTopologyConfigurator
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessagePublishTopology\<TMessage\>](../../masstransit-abstractions/masstransit-topology/messagepublishtopology-1) → [InMemoryMessagePublishTopology\<TMessage\>](../masstransit-inmemorytransport/inmemorymessagepublishtopology-1)<br/>
Implements [IMessagePublishTopologyConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/imessagepublishtopologyconfigurator-1), [IMessagePublishTopologyConfigurator](../../masstransit-abstractions/masstransit/imessagepublishtopologyconfigurator), [IMessagePublishTopology](../../masstransit-abstractions/masstransit/imessagepublishtopology), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IMessagePublishTopology\<TMessage\>](../../masstransit-abstractions/masstransit/imessagepublishtopology-1), [IInMemoryMessagePublishTopologyConfigurator\<TMessage\>](../masstransit/iinmemorymessagepublishtopologyconfigurator-1), [IInMemoryMessagePublishTopology\<TMessage\>](../masstransit/iinmemorymessagepublishtopology-1), [IInMemoryMessagePublishTopology](../masstransit/iinmemorymessagepublishtopology), [IInMemoryMessagePublishTopologyConfigurator](../masstransit/iinmemorymessagepublishtopologyconfigurator)

## Properties

### **ExchangeType**

```csharp
public ExchangeType ExchangeType { get; set; }
```

#### Property Value

[ExchangeType](../masstransit-transports-fabric/exchangetype)<br/>

### **Exclude**

```csharp
public bool Exclude { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **InMemoryMessagePublishTopology(IPublishTopologyConfigurator, IMessageTopology\<TMessage\>)**

```csharp
public InMemoryMessagePublishTopology(IPublishTopologyConfigurator publishTopology, IMessageTopology<TMessage> messageTopology)
```

#### Parameters

`publishTopology` [IPublishTopologyConfigurator](../../masstransit-abstractions/masstransit/ipublishtopologyconfigurator)<br/>

`messageTopology` [IMessageTopology\<TMessage\>](../../masstransit-abstractions/masstransit/imessagetopology-1)<br/>

## Methods

### **Apply(IMessageFabricPublishTopologyBuilder)**

```csharp
public void Apply(IMessageFabricPublishTopologyBuilder builder)
```

#### Parameters

`builder` [IMessageFabricPublishTopologyBuilder](../masstransit-configuration/imessagefabricpublishtopologybuilder)<br/>

### **TryGetPublishAddress(Uri, Uri)**

```csharp
public bool TryGetPublishAddress(Uri baseAddress, out Uri publishAddress)
```

#### Parameters

`baseAddress` Uri<br/>

`publishAddress` Uri<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **AddImplementedMessageConfigurator\<T\>(IInMemoryMessagePublishTopologyConfigurator\<T\>, Boolean)**

```csharp
public void AddImplementedMessageConfigurator<T>(IInMemoryMessagePublishTopologyConfigurator<T> configurator, bool direct)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IInMemoryMessagePublishTopologyConfigurator\<T\>](../masstransit/iinmemorymessagepublishtopologyconfigurator-1)<br/>

`direct` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
