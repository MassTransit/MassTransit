---

title: MessageSendPipeSpecification<TMessage>

---

# MessageSendPipeSpecification\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class MessageSendPipeSpecification<TMessage> : IMessageSendPipeSpecification<TMessage>, IPipeConfigurator<SendContext<TMessage>>, ISpecificationPipeSpecification<SendContext<TMessage>>, ISpecification, IMessageSendPipeSpecification, IPipeConfigurator<SendContext>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageSendPipeSpecification\<TMessage\>](../masstransit-configuration/messagesendpipespecification-1)<br/>
Implements [IMessageSendPipeSpecification\<TMessage\>](../masstransit-configuration/imessagesendpipespecification-1), [IPipeConfigurator\<SendContext\<TMessage\>\>](../masstransit/ipipeconfigurator-1), [ISpecificationPipeSpecification\<SendContext\<TMessage\>\>](../masstransit-configuration/ispecificationpipespecification-1), [ISpecification](../masstransit/ispecification), [IMessageSendPipeSpecification](../masstransit-configuration/imessagesendpipespecification), [IPipeConfigurator\<SendContext\>](../masstransit/ipipeconfigurator-1)

## Constructors

### **MessageSendPipeSpecification()**

```csharp
public MessageSendPipeSpecification()
```

## Methods

### **AddPipeSpecification(IPipeSpecification\<SendContext\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<SendContext> specification)
```

#### Parameters

`specification` [IPipeSpecification\<SendContext\>](../masstransit-configuration/ipipespecification-1)<br/>

### **AddPipeSpecification(IPipeSpecification\<SendContext\<TMessage\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<SendContext<TMessage>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<SendContext\<TMessage\>\>](../masstransit-configuration/ipipespecification-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Apply(ISpecificationPipeBuilder\<SendContext\<TMessage\>\>)**

```csharp
public void Apply(ISpecificationPipeBuilder<SendContext<TMessage>> builder)
```

#### Parameters

`builder` [ISpecificationPipeBuilder\<SendContext\<TMessage\>\>](../masstransit-configuration/ispecificationpipebuilder-1)<br/>

### **BuildMessagePipe()**

```csharp
public IPipe<SendContext<TMessage>> BuildMessagePipe()
```

#### Returns

[IPipe\<SendContext\<TMessage\>\>](../masstransit/ipipe-1)<br/>

### **AddParentMessageSpecification(ISpecificationPipeSpecification\<SendContext\<TMessage\>\>)**

```csharp
public void AddParentMessageSpecification(ISpecificationPipeSpecification<SendContext<TMessage>> parentSpecification)
```

#### Parameters

`parentSpecification` [ISpecificationPipeSpecification\<SendContext\<TMessage\>\>](../masstransit-configuration/ispecificationpipespecification-1)<br/>

### **AddImplementedMessageSpecification\<T\>(ISpecificationPipeSpecification\<SendContext\<T\>\>)**

```csharp
public void AddImplementedMessageSpecification<T>(ISpecificationPipeSpecification<SendContext<T>> implementedSpecification)
```

#### Type Parameters

`T`<br/>

#### Parameters

`implementedSpecification` [ISpecificationPipeSpecification\<SendContext\<T\>\>](../masstransit-configuration/ispecificationpipespecification-1)<br/>
