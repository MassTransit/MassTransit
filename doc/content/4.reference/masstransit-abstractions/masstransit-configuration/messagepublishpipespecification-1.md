---

title: MessagePublishPipeSpecification<TMessage>

---

# MessagePublishPipeSpecification\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class MessagePublishPipeSpecification<TMessage> : IMessagePublishPipeSpecification<TMessage>, IPipeConfigurator<PublishContext<TMessage>>, ISpecificationPipeSpecification<PublishContext<TMessage>>, ISpecification, IMessagePublishPipeSpecification, IPipeConfigurator<PublishContext>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessagePublishPipeSpecification\<TMessage\>](../masstransit-configuration/messagepublishpipespecification-1)<br/>
Implements [IMessagePublishPipeSpecification\<TMessage\>](../masstransit-configuration/imessagepublishpipespecification-1), [IPipeConfigurator\<PublishContext\<TMessage\>\>](../masstransit/ipipeconfigurator-1), [ISpecificationPipeSpecification\<PublishContext\<TMessage\>\>](../masstransit-configuration/ispecificationpipespecification-1), [ISpecification](../masstransit/ispecification), [IMessagePublishPipeSpecification](../masstransit-configuration/imessagepublishpipespecification), [IPipeConfigurator\<PublishContext\>](../masstransit/ipipeconfigurator-1)

## Constructors

### **MessagePublishPipeSpecification()**

```csharp
public MessagePublishPipeSpecification()
```

## Methods

### **AddPipeSpecification(IPipeSpecification\<PublishContext\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<PublishContext> specification)
```

#### Parameters

`specification` [IPipeSpecification\<PublishContext\>](../masstransit-configuration/ipipespecification-1)<br/>

### **AddPipeSpecification(IPipeSpecification\<PublishContext\<TMessage\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<PublishContext<TMessage>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<PublishContext\<TMessage\>\>](../masstransit-configuration/ipipespecification-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Apply(ISpecificationPipeBuilder\<PublishContext\<TMessage\>\>)**

```csharp
public void Apply(ISpecificationPipeBuilder<PublishContext<TMessage>> builder)
```

#### Parameters

`builder` [ISpecificationPipeBuilder\<PublishContext\<TMessage\>\>](../masstransit-configuration/ispecificationpipebuilder-1)<br/>

### **BuildMessagePipe()**

```csharp
public IPipe<PublishContext<TMessage>> BuildMessagePipe()
```

#### Returns

[IPipe\<PublishContext\<TMessage\>\>](../masstransit/ipipe-1)<br/>

### **AddParentMessageSpecification(ISpecificationPipeSpecification\<PublishContext\<TMessage\>\>)**

```csharp
public void AddParentMessageSpecification(ISpecificationPipeSpecification<PublishContext<TMessage>> implementedMessageTypeSpecification)
```

#### Parameters

`implementedMessageTypeSpecification` [ISpecificationPipeSpecification\<PublishContext\<TMessage\>\>](../masstransit-configuration/ispecificationpipespecification-1)<br/>

### **AddImplementedMessageSpecification\<T\>(ISpecificationPipeSpecification\<PublishContext\<T\>\>)**

```csharp
public void AddImplementedMessageSpecification<T>(ISpecificationPipeSpecification<PublishContext<T>> implementedMessageTypeSpecification)
```

#### Type Parameters

`T`<br/>

#### Parameters

`implementedMessageTypeSpecification` [ISpecificationPipeSpecification\<PublishContext\<T\>\>](../masstransit-configuration/ispecificationpipespecification-1)<br/>
