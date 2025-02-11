---

title: SetCorrelationIdMessageSendTopology<T>

---

# SetCorrelationIdMessageSendTopology\<T\>

Namespace: MassTransit.Topology

```csharp
public class SetCorrelationIdMessageSendTopology<T> : IMessageSendTopology<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SetCorrelationIdMessageSendTopology\<T\>](../masstransit-topology/setcorrelationidmessagesendtopology-1)<br/>
Implements [IMessageSendTopology\<T\>](../../masstransit-abstractions/masstransit/imessagesendtopology-1)

## Constructors

### **SetCorrelationIdMessageSendTopology(IMessageCorrelationId\<T\>)**

```csharp
public SetCorrelationIdMessageSendTopology(IMessageCorrelationId<T> messageCorrelationId)
```

#### Parameters

`messageCorrelationId` [IMessageCorrelationId\<T\>](../masstransit/imessagecorrelationid-1)<br/>

## Methods

### **Apply(ITopologyPipeBuilder\<SendContext\<T\>\>)**

```csharp
public void Apply(ITopologyPipeBuilder<SendContext<T>> builder)
```

#### Parameters

`builder` [ITopologyPipeBuilder\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/itopologypipebuilder-1)<br/>
