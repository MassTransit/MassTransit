---

title: IMessagePublishPipeSpecification<TMessage>

---

# IMessagePublishPipeSpecification\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public interface IMessagePublishPipeSpecification<TMessage> : IPipeConfigurator<PublishContext<TMessage>>, ISpecificationPipeSpecification<PublishContext<TMessage>>, ISpecification
```

#### Type Parameters

`TMessage`<br/>

Implements [IPipeConfigurator\<PublishContext\<TMessage\>\>](../masstransit/ipipeconfigurator-1), [ISpecificationPipeSpecification\<PublishContext\<TMessage\>\>](../masstransit-configuration/ispecificationpipespecification-1), [ISpecification](../masstransit/ispecification)

## Methods

### **AddParentMessageSpecification(ISpecificationPipeSpecification\<PublishContext\<TMessage\>\>)**

```csharp
void AddParentMessageSpecification(ISpecificationPipeSpecification<PublishContext<TMessage>> implementedMessageTypeSpecification)
```

#### Parameters

`implementedMessageTypeSpecification` [ISpecificationPipeSpecification\<PublishContext\<TMessage\>\>](../masstransit-configuration/ispecificationpipespecification-1)<br/>

### **BuildMessagePipe()**

Build the pipe for the specification

```csharp
IPipe<PublishContext<TMessage>> BuildMessagePipe()
```

#### Returns

[IPipe\<PublishContext\<TMessage\>\>](../masstransit/ipipe-1)<br/>
