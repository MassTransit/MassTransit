---

title: SetSerializerMessageSendTopology<T>

---

# SetSerializerMessageSendTopology\<T\>

Namespace: MassTransit.Topology

```csharp
public class SetSerializerMessageSendTopology<T> : IMessageSendTopology<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SetSerializerMessageSendTopology\<T\>](../masstransit-topology/setserializermessagesendtopology-1)<br/>
Implements [IMessageSendTopology\<T\>](../../masstransit-abstractions/masstransit/imessagesendtopology-1)

## Constructors

### **SetSerializerMessageSendTopology(ContentType)**

```csharp
public SetSerializerMessageSendTopology(ContentType contentType)
```

#### Parameters

`contentType` ContentType<br/>

## Methods

### **Apply(ITopologyPipeBuilder\<SendContext\<T\>\>)**

```csharp
public void Apply(ITopologyPipeBuilder<SendContext<T>> builder)
```

#### Parameters

`builder` [ITopologyPipeBuilder\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/itopologypipebuilder-1)<br/>
