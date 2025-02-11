---

title: PublishPipeSpecification

---

# PublishPipeSpecification

Namespace: MassTransit.Configuration

```csharp
public class PublishPipeSpecification : IPublishPipeConfigurator, IPipeConfigurator<PublishContext>, IPublishPipeSpecificationObserverConnector, IPublishPipeSpecification, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishPipeSpecification](../masstransit-configuration/publishpipespecification)<br/>
Implements [IPublishPipeConfigurator](../masstransit/ipublishpipeconfigurator), [IPipeConfigurator\<PublishContext\>](../masstransit/ipipeconfigurator-1), [IPublishPipeSpecificationObserverConnector](../masstransit-configuration/ipublishpipespecificationobserverconnector), [IPublishPipeSpecification](../masstransit-configuration/ipublishpipespecification), [ISpecification](../masstransit/ispecification)

## Constructors

### **PublishPipeSpecification()**

```csharp
public PublishPipeSpecification()
```

## Methods

### **AddPipeSpecification(IPipeSpecification\<PublishContext\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<PublishContext> specification)
```

#### Parameters

`specification` [IPipeSpecification\<PublishContext\>](../masstransit-configuration/ipipespecification-1)<br/>

### **AddPipeSpecification\<T\>(IPipeSpecification\<PublishContext\<T\>\>)**

```csharp
public void AddPipeSpecification<T>(IPipeSpecification<PublishContext<T>> specification)
```

#### Type Parameters

`T`<br/>

#### Parameters

`specification` [IPipeSpecification\<PublishContext\<T\>\>](../masstransit-configuration/ipipespecification-1)<br/>

### **ConnectPublishPipeSpecificationObserver(IPublishPipeSpecificationObserver)**

```csharp
public ConnectHandle ConnectPublishPipeSpecificationObserver(IPublishPipeSpecificationObserver observer)
```

#### Parameters

`observer` [IPublishPipeSpecificationObserver](../masstransit-configuration/ipublishpipespecificationobserver)<br/>

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
public IMessagePublishPipeSpecification<T> GetMessageSpecification<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IMessagePublishPipeSpecification\<T\>](../masstransit-configuration/imessagepublishpipespecification-1)<br/>
