---

title: MessageSendTopologyPipeSpecification<TMessage>

---

# MessageSendTopologyPipeSpecification\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class MessageSendTopologyPipeSpecification<TMessage> : ISpecificationPipeSpecification<SendContext<TMessage>>, ISpecification
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageSendTopologyPipeSpecification\<TMessage\>](../masstransit-configuration/messagesendtopologypipespecification-1)<br/>
Implements [ISpecificationPipeSpecification\<SendContext\<TMessage\>\>](../masstransit-configuration/ispecificationpipespecification-1), [ISpecification](../masstransit/ispecification)

## Constructors

### **MessageSendTopologyPipeSpecification(IMessageSendTopology\<TMessage\>)**

```csharp
public MessageSendTopologyPipeSpecification(IMessageSendTopology<TMessage> messageSendTopology)
```

#### Parameters

`messageSendTopology` [IMessageSendTopology\<TMessage\>](../masstransit/imessagesendtopology-1)<br/>

## Methods

### **Apply(ISpecificationPipeBuilder\<SendContext\<TMessage\>\>)**

```csharp
public void Apply(ISpecificationPipeBuilder<SendContext<TMessage>> builder)
```

#### Parameters

`builder` [ISpecificationPipeBuilder\<SendContext\<TMessage\>\>](../masstransit-configuration/ispecificationpipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
