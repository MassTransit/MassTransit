---

title: ISendPipeSpecification

---

# ISendPipeSpecification

Namespace: MassTransit.Configuration

```csharp
public interface ISendPipeSpecification : ISendPipeSpecificationObserverConnector, ISpecification
```

Implements [ISendPipeSpecificationObserverConnector](../masstransit-configuration/isendpipespecificationobserverconnector), [ISpecification](../masstransit/ispecification)

## Methods

### **GetMessageSpecification\<T\>()**

Returns the specification for the message type

```csharp
IMessageSendPipeSpecification<T> GetMessageSpecification<T>()
```

#### Type Parameters

`T`<br/>
The message type

#### Returns

[IMessageSendPipeSpecification\<T\>](../masstransit-configuration/imessagesendpipespecification-1)<br/>
