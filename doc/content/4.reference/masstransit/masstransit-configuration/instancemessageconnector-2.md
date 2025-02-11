---

title: InstanceMessageConnector<TConsumer, TMessage>

---

# InstanceMessageConnector\<TConsumer, TMessage\>

Namespace: MassTransit.Configuration

Connects a consumer instance to the inbound pipeline for the specified message type. The actual
 filter that invokes the consume method is passed to allow different types of message bindings,
 including the legacy bindings from v2.x

```csharp
public class InstanceMessageConnector<TConsumer, TMessage> : IInstanceMessageConnector<TConsumer>, IInstanceMessageConnector
```

#### Type Parameters

`TConsumer`<br/>
The consumer type

`TMessage`<br/>
The message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InstanceMessageConnector\<TConsumer, TMessage\>](../masstransit-configuration/instancemessageconnector-2)<br/>
Implements [IInstanceMessageConnector\<TConsumer\>](../masstransit-configuration/iinstancemessageconnector-1), [IInstanceMessageConnector](../masstransit-configuration/iinstancemessageconnector)

## Constructors

### **InstanceMessageConnector(IFilter\<ConsumerConsumeContext\<TConsumer, TMessage\>\>)**

Constructs the instance connector

```csharp
public InstanceMessageConnector(IFilter<ConsumerConsumeContext<TConsumer, TMessage>> consumeFilter)
```

#### Parameters

`consumeFilter` [IFilter\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1)<br/>
The consume method invocation filter

## Methods

### **ConnectInstance(IConsumePipeConnector, TConsumer, IConsumerSpecification\<TConsumer\>)**

```csharp
public ConnectHandle ConnectInstance(IConsumePipeConnector pipeConnector, TConsumer instance, IConsumerSpecification<TConsumer> specification)
```

#### Parameters

`pipeConnector` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`instance` TConsumer<br/>

`specification` [IConsumerSpecification\<TConsumer\>](../masstransit-configuration/iconsumerspecification-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **CreateConsumerMessageSpecification()**

```csharp
public IConsumerMessageSpecification<TConsumer> CreateConsumerMessageSpecification()
```

#### Returns

[IConsumerMessageSpecification\<TConsumer\>](../masstransit-configuration/iconsumermessagespecification-1)<br/>
