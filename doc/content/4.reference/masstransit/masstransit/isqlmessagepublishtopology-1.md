---

title: ISqlMessagePublishTopology<TMessage>

---

# ISqlMessagePublishTopology\<TMessage\>

Namespace: MassTransit

```csharp
public interface ISqlMessagePublishTopology<TMessage> : IMessagePublishTopology<TMessage>, IMessagePublishTopology, ISqlMessagePublishTopology
```

#### Type Parameters

`TMessage`<br/>

Implements [IMessagePublishTopology\<TMessage\>](../../masstransit-abstractions/masstransit/imessagepublishtopology-1), [IMessagePublishTopology](../../masstransit-abstractions/masstransit/imessagepublishtopology), [ISqlMessagePublishTopology](../masstransit/isqlmessagepublishtopology)

## Properties

### **Topic**

```csharp
public abstract Topic Topic { get; }
```

#### Property Value

[Topic](../masstransit-sqltransport-topology/topic)<br/>

## Methods

### **GetSendSettings(Uri)**

```csharp
SendSettings GetSendSettings(Uri hostAddress)
```

#### Parameters

`hostAddress` Uri<br/>

#### Returns

[SendSettings](../masstransit-sqltransport/sendsettings)<br/>

### **GetBrokerTopology()**

```csharp
BrokerTopology GetBrokerTopology()
```

#### Returns

[BrokerTopology](../masstransit-sqltransport-topology/brokertopology)<br/>
