---

title: IConsumePipeSpecification

---

# IConsumePipeSpecification

Namespace: MassTransit.Configuration

```csharp
public interface IConsumePipeSpecification : IConsumePipeSpecificationObserverConnector, ISpecification
```

Implements [IConsumePipeSpecificationObserverConnector](../masstransit-configuration/iconsumepipespecificationobserverconnector), [ISpecification](../masstransit/ispecification)

## Methods

### **GetMessageSpecification\<T\>()**

Returns the specification for the message type

```csharp
IMessageConsumePipeSpecification<T> GetMessageSpecification<T>()
```

#### Type Parameters

`T`<br/>
The message type

#### Returns

[IMessageConsumePipeSpecification\<T\>](../masstransit-configuration/imessageconsumepipespecification-1)<br/>

### **BuildConsumePipe()**

Build the consume pipe for the specification

```csharp
IConsumePipe BuildConsumePipe()
```

#### Returns

[IConsumePipe](../masstransit-transports/iconsumepipe)<br/>

### **CreateConsumePipeSpecification()**

```csharp
IConsumePipeSpecification CreateConsumePipeSpecification()
```

#### Returns

[IConsumePipeSpecification](../masstransit-configuration/iconsumepipespecification)<br/>
