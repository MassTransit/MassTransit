---

title: IMessageSendPipeSpecification

---

# IMessageSendPipeSpecification

Namespace: MassTransit.Configuration

```csharp
public interface IMessageSendPipeSpecification : IPipeConfigurator<SendContext>, ISpecification
```

Implements [IPipeConfigurator\<SendContext\>](../masstransit/ipipeconfigurator-1), [ISpecification](../masstransit/ispecification)

## Methods

### **GetMessageSpecification\<T\>()**

```csharp
IMessageSendPipeSpecification<T> GetMessageSpecification<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IMessageSendPipeSpecification\<T\>](../masstransit-configuration/imessagesendpipespecification-1)<br/>
