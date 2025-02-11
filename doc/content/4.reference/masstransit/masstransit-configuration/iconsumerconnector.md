---

title: IConsumerConnector

---

# IConsumerConnector

Namespace: MassTransit.Configuration

Interface implemented by objects that tie an inbound pipeline together with
 consumers (by means of calling a consumer factory).

```csharp
public interface IConsumerConnector
```

## Methods

### **CreateConsumerSpecification\<TConsumer\>()**

```csharp
IConsumerSpecification<TConsumer> CreateConsumerSpecification<TConsumer>()
```

#### Type Parameters

`TConsumer`<br/>

#### Returns

[IConsumerSpecification\<TConsumer\>](../masstransit-configuration/iconsumerspecification-1)<br/>

### **ConnectConsumer\<TConsumer\>(IConsumePipeConnector, IConsumerFactory\<TConsumer\>, IConsumerSpecification\<TConsumer\>)**

```csharp
ConnectHandle ConnectConsumer<TConsumer>(IConsumePipeConnector consumePipe, IConsumerFactory<TConsumer> consumerFactory, IConsumerSpecification<TConsumer> specification)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`consumePipe` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`consumerFactory` [IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1)<br/>

`specification` [IConsumerSpecification\<TConsumer\>](../masstransit-configuration/iconsumerspecification-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
