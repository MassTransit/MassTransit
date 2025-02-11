---

title: ParentConsumePipeSpecificationObserver

---

# ParentConsumePipeSpecificationObserver

Namespace: MassTransit.Configuration

```csharp
public class ParentConsumePipeSpecificationObserver : IConsumePipeSpecificationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ParentConsumePipeSpecificationObserver](../masstransit-configuration/parentconsumepipespecificationobserver)<br/>
Implements [IConsumePipeSpecificationObserver](../masstransit-configuration/iconsumepipespecificationobserver)

## Constructors

### **ParentConsumePipeSpecificationObserver(IConsumePipeSpecification)**

```csharp
public ParentConsumePipeSpecificationObserver(IConsumePipeSpecification specification)
```

#### Parameters

`specification` [IConsumePipeSpecification](../masstransit-configuration/iconsumepipespecification)<br/>

## Methods

### **MessageSpecificationCreated\<T\>(IMessageConsumePipeSpecification\<T\>)**

```csharp
public void MessageSpecificationCreated<T>(IMessageConsumePipeSpecification<T> specification)
```

#### Type Parameters

`T`<br/>

#### Parameters

`specification` [IMessageConsumePipeSpecification\<T\>](../masstransit-configuration/imessageconsumepipespecification-1)<br/>
