---

title: IMessagePublishPipeSpecification

---

# IMessagePublishPipeSpecification

Namespace: MassTransit.Configuration

```csharp
public interface IMessagePublishPipeSpecification : IPipeConfigurator<PublishContext>, ISpecification
```

Implements [IPipeConfigurator\<PublishContext\>](../masstransit/ipipeconfigurator-1), [ISpecification](../masstransit/ispecification)

## Methods

### **GetMessageSpecification\<T\>()**

```csharp
IMessagePublishPipeSpecification<T> GetMessageSpecification<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IMessagePublishPipeSpecification\<T\>](../masstransit-configuration/imessagepublishpipespecification-1)<br/>
