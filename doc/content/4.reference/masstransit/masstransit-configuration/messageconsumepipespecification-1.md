---

title: MessageConsumePipeSpecification<TMessage>

---

# MessageConsumePipeSpecification\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class MessageConsumePipeSpecification<TMessage> : IMessageConsumePipeSpecification<TMessage>, IMessageConsumePipeConfigurator<TMessage>, IPipeConfigurator<ConsumeContext<TMessage>>, ISpecificationPipeSpecification<ConsumeContext<TMessage>>, ISpecification, IMessageConsumePipeSpecification, IPipeConfigurator<ConsumeContext>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageConsumePipeSpecification\<TMessage\>](../masstransit-configuration/messageconsumepipespecification-1)<br/>
Implements [IMessageConsumePipeSpecification\<TMessage\>](../../masstransit-abstractions/masstransit-configuration/imessageconsumepipespecification-1), [IMessageConsumePipeConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit-configuration/imessageconsumepipeconfigurator-1), [IPipeConfigurator\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [ISpecificationPipeSpecification\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ispecificationpipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IMessageConsumePipeSpecification](../../masstransit-abstractions/masstransit-configuration/imessageconsumepipespecification), [IPipeConfigurator\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)

## Constructors

### **MessageConsumePipeSpecification()**

```csharp
public MessageConsumePipeSpecification()
```

## Methods

### **AddPipeSpecification(IPipeSpecification\<ConsumeContext\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ConsumeContext\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **AddPipeSpecification(IPipeSpecification\<ConsumeContext\<TMessage\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<ConsumeContext<TMessage>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Apply(ISpecificationPipeBuilder\<ConsumeContext\<TMessage\>\>)**

```csharp
public void Apply(ISpecificationPipeBuilder<ConsumeContext<TMessage>> builder)
```

#### Parameters

`builder` [ISpecificationPipeBuilder\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ispecificationpipebuilder-1)<br/>

### **BuildMessagePipe(IPipe\<ConsumeContext\<TMessage\>\>)**

```csharp
public IPipe<ConsumeContext<TMessage>> BuildMessagePipe(IPipe<ConsumeContext<TMessage>> pipe)
```

#### Parameters

`pipe` [IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

### **AddParentMessageSpecification(ISpecificationPipeSpecification\<ConsumeContext\<TMessage\>\>)**

```csharp
public void AddParentMessageSpecification(ISpecificationPipeSpecification<ConsumeContext<TMessage>> parentSpecification)
```

#### Parameters

`parentSpecification` [ISpecificationPipeSpecification\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ispecificationpipespecification-1)<br/>
