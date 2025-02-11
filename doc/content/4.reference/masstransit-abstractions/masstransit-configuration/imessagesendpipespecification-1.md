---

title: IMessageSendPipeSpecification<TMessage>

---

# IMessageSendPipeSpecification\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public interface IMessageSendPipeSpecification<TMessage> : IPipeConfigurator<SendContext<TMessage>>, ISpecificationPipeSpecification<SendContext<TMessage>>, ISpecification
```

#### Type Parameters

`TMessage`<br/>

Implements [IPipeConfigurator\<SendContext\<TMessage\>\>](../masstransit/ipipeconfigurator-1), [ISpecificationPipeSpecification\<SendContext\<TMessage\>\>](../masstransit-configuration/ispecificationpipespecification-1), [ISpecification](../masstransit/ispecification)

## Methods

### **AddParentMessageSpecification(ISpecificationPipeSpecification\<SendContext\<TMessage\>\>)**

```csharp
void AddParentMessageSpecification(ISpecificationPipeSpecification<SendContext<TMessage>> parentSpecification)
```

#### Parameters

`parentSpecification` [ISpecificationPipeSpecification\<SendContext\<TMessage\>\>](../masstransit-configuration/ispecificationpipespecification-1)<br/>

### **BuildMessagePipe()**

Build the pipe for the specification

```csharp
IPipe<SendContext<TMessage>> BuildMessagePipe()
```

#### Returns

[IPipe\<SendContext\<TMessage\>\>](../masstransit/ipipe-1)<br/>
