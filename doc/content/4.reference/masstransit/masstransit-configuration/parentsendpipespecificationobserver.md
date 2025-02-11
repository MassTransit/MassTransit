---

title: ParentSendPipeSpecificationObserver

---

# ParentSendPipeSpecificationObserver

Namespace: MassTransit.Configuration

```csharp
public class ParentSendPipeSpecificationObserver : ISendPipeSpecificationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ParentSendPipeSpecificationObserver](../masstransit-configuration/parentsendpipespecificationobserver)<br/>
Implements [ISendPipeSpecificationObserver](../../masstransit-abstractions/masstransit-configuration/isendpipespecificationobserver)

## Constructors

### **ParentSendPipeSpecificationObserver(ISendPipeSpecification)**

```csharp
public ParentSendPipeSpecificationObserver(ISendPipeSpecification specification)
```

#### Parameters

`specification` [ISendPipeSpecification](../../masstransit-abstractions/masstransit-configuration/isendpipespecification)<br/>

## Methods

### **MessageSpecificationCreated\<T\>(IMessageSendPipeSpecification\<T\>)**

```csharp
public void MessageSpecificationCreated<T>(IMessageSendPipeSpecification<T> specification)
```

#### Type Parameters

`T`<br/>

#### Parameters

`specification` [IMessageSendPipeSpecification\<T\>](../../masstransit-abstractions/masstransit-configuration/imessagesendpipespecification-1)<br/>
