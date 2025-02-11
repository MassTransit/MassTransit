---

title: ParentPublishPipeSpecificationObserver

---

# ParentPublishPipeSpecificationObserver

Namespace: MassTransit.Configuration

```csharp
public class ParentPublishPipeSpecificationObserver : IPublishPipeSpecificationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ParentPublishPipeSpecificationObserver](../masstransit-configuration/parentpublishpipespecificationobserver)<br/>
Implements [IPublishPipeSpecificationObserver](../../masstransit-abstractions/masstransit-configuration/ipublishpipespecificationobserver)

## Constructors

### **ParentPublishPipeSpecificationObserver(IPublishPipeSpecification)**

```csharp
public ParentPublishPipeSpecificationObserver(IPublishPipeSpecification specification)
```

#### Parameters

`specification` [IPublishPipeSpecification](../../masstransit-abstractions/masstransit-configuration/ipublishpipespecification)<br/>

## Methods

### **MessageSpecificationCreated\<T\>(IMessagePublishPipeSpecification\<T\>)**

```csharp
public void MessageSpecificationCreated<T>(IMessagePublishPipeSpecification<T> specification)
```

#### Type Parameters

`T`<br/>

#### Parameters

`specification` [IMessagePublishPipeSpecification\<T\>](../../masstransit-abstractions/masstransit-configuration/imessagepublishpipespecification-1)<br/>
