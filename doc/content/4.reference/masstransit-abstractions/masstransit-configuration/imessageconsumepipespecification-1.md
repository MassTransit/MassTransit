---

title: IMessageConsumePipeSpecification<TMessage>

---

# IMessageConsumePipeSpecification\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public interface IMessageConsumePipeSpecification<TMessage> : IMessageConsumePipeConfigurator<TMessage>, IPipeConfigurator<ConsumeContext<TMessage>>, ISpecificationPipeSpecification<ConsumeContext<TMessage>>, ISpecification
```

#### Type Parameters

`TMessage`<br/>

Implements [IMessageConsumePipeConfigurator\<TMessage\>](../masstransit-configuration/imessageconsumepipeconfigurator-1), [IPipeConfigurator\<ConsumeContext\<TMessage\>\>](../masstransit/ipipeconfigurator-1), [ISpecificationPipeSpecification\<ConsumeContext\<TMessage\>\>](../masstransit-configuration/ispecificationpipespecification-1), [ISpecification](../masstransit/ispecification)

## Methods

### **AddParentMessageSpecification(ISpecificationPipeSpecification\<ConsumeContext\<TMessage\>\>)**

```csharp
void AddParentMessageSpecification(ISpecificationPipeSpecification<ConsumeContext<TMessage>> parentSpecification)
```

#### Parameters

`parentSpecification` [ISpecificationPipeSpecification\<ConsumeContext\<TMessage\>\>](../masstransit-configuration/ispecificationpipespecification-1)<br/>

### **BuildMessagePipe(IPipe\<ConsumeContext\<TMessage\>\>)**

```csharp
IPipe<ConsumeContext<TMessage>> BuildMessagePipe(IPipe<ConsumeContext<TMessage>> pipe)
```

#### Parameters

`pipe` [IPipe\<ConsumeContext\<TMessage\>\>](../masstransit/ipipe-1)<br/>

#### Returns

[IPipe\<ConsumeContext\<TMessage\>\>](../masstransit/ipipe-1)<br/>
