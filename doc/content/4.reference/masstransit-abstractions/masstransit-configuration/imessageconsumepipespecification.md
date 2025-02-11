---

title: IMessageConsumePipeSpecification

---

# IMessageConsumePipeSpecification

Namespace: MassTransit.Configuration

```csharp
public interface IMessageConsumePipeSpecification : IPipeConfigurator<ConsumeContext>, ISpecification
```

Implements [IPipeConfigurator\<ConsumeContext\>](../masstransit/ipipeconfigurator-1), [ISpecification](../masstransit/ispecification)

## Methods

### **GetMessageSpecification\<T\>()**

```csharp
IMessageConsumePipeSpecification<T> GetMessageSpecification<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IMessageConsumePipeSpecification\<T\>](../masstransit-configuration/imessageconsumepipespecification-1)<br/>
