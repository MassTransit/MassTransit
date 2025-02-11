---

title: IPublishPipeSpecification

---

# IPublishPipeSpecification

Namespace: MassTransit.Configuration

```csharp
public interface IPublishPipeSpecification : IPublishPipeSpecificationObserverConnector, ISpecification
```

Implements [IPublishPipeSpecificationObserverConnector](../masstransit-configuration/ipublishpipespecificationobserverconnector), [ISpecification](../masstransit/ispecification)

## Methods

### **GetMessageSpecification\<T\>()**

Returns the specification for the message type

```csharp
IMessagePublishPipeSpecification<T> GetMessageSpecification<T>()
```

#### Type Parameters

`T`<br/>
The message type

#### Returns

[IMessagePublishPipeSpecification\<T\>](../masstransit-configuration/imessagepublishpipespecification-1)<br/>
