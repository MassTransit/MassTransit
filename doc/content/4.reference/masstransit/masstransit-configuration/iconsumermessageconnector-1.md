---

title: IConsumerMessageConnector<TConsumer>

---

# IConsumerMessageConnector\<TConsumer\>

Namespace: MassTransit.Configuration

```csharp
public interface IConsumerMessageConnector<TConsumer> : IConsumerMessageConnector
```

#### Type Parameters

`TConsumer`<br/>

Implements [IConsumerMessageConnector](../masstransit-configuration/iconsumermessageconnector)

## Methods

### **CreateConsumerMessageSpecification()**

```csharp
IConsumerMessageSpecification<TConsumer> CreateConsumerMessageSpecification()
```

#### Returns

[IConsumerMessageSpecification\<TConsumer\>](../masstransit-configuration/iconsumermessagespecification-1)<br/>

### **ConnectConsumer(IConsumePipeConnector, IConsumerFactory\<TConsumer\>, IConsumerSpecification\<TConsumer\>)**

```csharp
ConnectHandle ConnectConsumer(IConsumePipeConnector consumePipe, IConsumerFactory<TConsumer> consumerFactory, IConsumerSpecification<TConsumer> specification)
```

#### Parameters

`consumePipe` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`consumerFactory` [IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1)<br/>

`specification` [IConsumerSpecification\<TConsumer\>](../masstransit-configuration/iconsumerspecification-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
