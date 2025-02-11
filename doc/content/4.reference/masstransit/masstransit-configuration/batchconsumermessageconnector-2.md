---

title: BatchConsumerMessageConnector<TConsumer, TMessage>

---

# BatchConsumerMessageConnector\<TConsumer, TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class BatchConsumerMessageConnector<TConsumer, TMessage> : IConsumerMessageConnector<TConsumer>, IConsumerMessageConnector
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BatchConsumerMessageConnector\<TConsumer, TMessage\>](../masstransit-configuration/batchconsumermessageconnector-2)<br/>
Implements [IConsumerMessageConnector\<TConsumer\>](../masstransit-configuration/iconsumermessageconnector-1), [IConsumerMessageConnector](../masstransit-configuration/iconsumermessageconnector)

## Properties

### **MessageType**

```csharp
public Type MessageType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

## Constructors

### **BatchConsumerMessageConnector()**

```csharp
public BatchConsumerMessageConnector()
```

## Methods

### **CreateConsumerMessageSpecification()**

```csharp
public IConsumerMessageSpecification<TConsumer> CreateConsumerMessageSpecification()
```

#### Returns

[IConsumerMessageSpecification\<TConsumer\>](../masstransit-configuration/iconsumermessagespecification-1)<br/>

### **ConnectConsumer(IConsumePipeConnector, IConsumerFactory\<TConsumer\>, IConsumerSpecification\<TConsumer\>)**

```csharp
public ConnectHandle ConnectConsumer(IConsumePipeConnector consumePipe, IConsumerFactory<TConsumer> consumerFactory, IConsumerSpecification<TConsumer> specification)
```

#### Parameters

`consumePipe` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`consumerFactory` [IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1)<br/>

`specification` [IConsumerSpecification\<TConsumer\>](../masstransit-configuration/iconsumerspecification-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
