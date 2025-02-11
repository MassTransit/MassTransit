---

title: SendPipeSpecification

---

# SendPipeSpecification

Namespace: MassTransit.Configuration

```csharp
public class SendPipeSpecification : ISendPipeConfigurator, IPipeConfigurator<SendContext>, ISendPipeSpecificationObserverConnector, ISendPipeSpecification, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendPipeSpecification](../masstransit-configuration/sendpipespecification)<br/>
Implements [ISendPipeConfigurator](../masstransit/isendpipeconfigurator), [IPipeConfigurator\<SendContext\>](../masstransit/ipipeconfigurator-1), [ISendPipeSpecificationObserverConnector](../masstransit-configuration/isendpipespecificationobserverconnector), [ISendPipeSpecification](../masstransit-configuration/isendpipespecification), [ISpecification](../masstransit/ispecification)

## Constructors

### **SendPipeSpecification()**

```csharp
public SendPipeSpecification()
```

## Methods

### **AddPipeSpecification(IPipeSpecification\<SendContext\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<SendContext> specification)
```

#### Parameters

`specification` [IPipeSpecification\<SendContext\>](../masstransit-configuration/ipipespecification-1)<br/>

### **ConnectSendPipeSpecificationObserver(ISendPipeSpecificationObserver)**

```csharp
public ConnectHandle ConnectSendPipeSpecificationObserver(ISendPipeSpecificationObserver observer)
```

#### Parameters

`observer` [ISendPipeSpecificationObserver](../masstransit-configuration/isendpipespecificationobserver)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **GetMessageSpecification\<T\>()**

```csharp
public IMessageSendPipeSpecification<T> GetMessageSpecification<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IMessageSendPipeSpecification\<T\>](../masstransit-configuration/imessagesendpipespecification-1)<br/>
