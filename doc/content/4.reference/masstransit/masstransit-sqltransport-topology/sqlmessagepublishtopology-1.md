---

title: SqlMessagePublishTopology<TMessage>

---

# SqlMessagePublishTopology\<TMessage\>

Namespace: MassTransit.SqlTransport.Topology

```csharp
public class SqlMessagePublishTopology<TMessage> : MessagePublishTopology<TMessage>, IMessagePublishTopologyConfigurator<TMessage>, IMessagePublishTopologyConfigurator, IMessagePublishTopology, ISpecification, IMessagePublishTopology<TMessage>, ISqlMessagePublishTopologyConfigurator<TMessage>, ISqlMessagePublishTopology<TMessage>, ISqlMessagePublishTopology, ISqlMessagePublishTopologyConfigurator, ISqlTopicConfigurator
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessagePublishTopology\<TMessage\>](../../masstransit-abstractions/masstransit-topology/messagepublishtopology-1) → [SqlMessagePublishTopology\<TMessage\>](../masstransit-sqltransport-topology/sqlmessagepublishtopology-1)<br/>
Implements [IMessagePublishTopologyConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/imessagepublishtopologyconfigurator-1), [IMessagePublishTopologyConfigurator](../../masstransit-abstractions/masstransit/imessagepublishtopologyconfigurator), [IMessagePublishTopology](../../masstransit-abstractions/masstransit/imessagepublishtopology), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IMessagePublishTopology\<TMessage\>](../../masstransit-abstractions/masstransit/imessagepublishtopology-1), [ISqlMessagePublishTopologyConfigurator\<TMessage\>](../masstransit/isqlmessagepublishtopologyconfigurator-1), [ISqlMessagePublishTopology\<TMessage\>](../masstransit/isqlmessagepublishtopology-1), [ISqlMessagePublishTopology](../masstransit/isqlmessagepublishtopology), [ISqlMessagePublishTopologyConfigurator](../masstransit/isqlmessagepublishtopologyconfigurator), [ISqlTopicConfigurator](../masstransit/isqltopicconfigurator)

## Properties

### **Topic**

```csharp
public Topic Topic { get; }
```

#### Property Value

[Topic](../masstransit-sqltransport-topology/topic)<br/>

### **Exclude**

```csharp
public bool Exclude { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **SqlMessagePublishTopology(ISqlPublishTopology, IMessageTopology\<TMessage\>)**

```csharp
public SqlMessagePublishTopology(ISqlPublishTopology publishTopology, IMessageTopology<TMessage> messageTopology)
```

#### Parameters

`publishTopology` [ISqlPublishTopology](../masstransit/isqlpublishtopology)<br/>

`messageTopology` [IMessageTopology\<TMessage\>](../../masstransit-abstractions/masstransit/imessagetopology-1)<br/>

## Methods

### **Apply(IPublishEndpointBrokerTopologyBuilder)**

```csharp
public void Apply(IPublishEndpointBrokerTopologyBuilder builder)
```

#### Parameters

`builder` [IPublishEndpointBrokerTopologyBuilder](../masstransit-sqltransport-topology/ipublishendpointbrokertopologybuilder)<br/>

### **TryGetPublishAddress(Uri, Uri)**

```csharp
public bool TryGetPublishAddress(Uri baseAddress, out Uri publishAddress)
```

#### Parameters

`baseAddress` Uri<br/>

`publishAddress` Uri<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetBrokerTopology()**

```csharp
public BrokerTopology GetBrokerTopology()
```

#### Returns

[BrokerTopology](../masstransit-sqltransport-topology/brokertopology)<br/>

### **GetSendSettings(Uri)**

```csharp
public SendSettings GetSendSettings(Uri hostAddress)
```

#### Parameters

`hostAddress` Uri<br/>

#### Returns

[SendSettings](../masstransit-sqltransport/sendsettings)<br/>

### **AddImplementedMessageConfigurator\<T\>(ISqlMessagePublishTopologyConfigurator\<T\>, Boolean)**

```csharp
public void AddImplementedMessageConfigurator<T>(ISqlMessagePublishTopologyConfigurator<T> configurator, bool direct)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [ISqlMessagePublishTopologyConfigurator\<T\>](../masstransit/isqlmessagepublishtopologyconfigurator-1)<br/>

`direct` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
