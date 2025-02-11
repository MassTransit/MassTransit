---

title: ConsumerMessageSpecification<TConsumer, TMessage>

---

# ConsumerMessageSpecification\<TConsumer, TMessage\>

Namespace: MassTransit.Configuration

Configures the pipe for a consumer/message combination within a consumer configuration
 block. Does not add any handlers to the message pipe standalone, everything is within
 the consumer pipe segment.

```csharp
public class ConsumerMessageSpecification<TConsumer, TMessage> : IConsumerMessageSpecification<TConsumer, TMessage>, IConsumerMessageSpecification<TConsumer>, IPipeConfigurator<ConsumerConsumeContext<TConsumer>>, IConsumerConfigurationObserverConnector, ISpecification, IConsumerMessageConfigurator<TConsumer, TMessage>, IPipeConfigurator<ConsumerConsumeContext<TConsumer, TMessage>>, IConsumerMessageConfigurator<TMessage>, IPipeConfigurator<ConsumeContext<TMessage>>
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerMessageSpecification\<TConsumer, TMessage\>](../masstransit-configuration/consumermessagespecification-2)<br/>
Implements [IConsumerMessageSpecification\<TConsumer, TMessage\>](../masstransit-configuration/iconsumermessagespecification-2), [IConsumerMessageSpecification\<TConsumer\>](../masstransit-configuration/iconsumermessagespecification-1), [IPipeConfigurator\<ConsumerConsumeContext\<TConsumer\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IConsumerMessageConfigurator\<TConsumer, TMessage\>](../../masstransit-abstractions/masstransit/iconsumermessageconfigurator-2), [IPipeConfigurator\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerMessageConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/iconsumermessageconfigurator-1), [IPipeConfigurator\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)

## Properties

### **MessageType**

```csharp
public Type MessageType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

## Constructors

### **ConsumerMessageSpecification()**

```csharp
public ConsumerMessageSpecification()
```

## Methods

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **TryGetMessageSpecification\<TC, T\>(IConsumerMessageSpecification\<TC, T\>)**

```csharp
public bool TryGetMessageSpecification<TC, T>(out IConsumerMessageSpecification<TC, T> specification)
```

#### Type Parameters

`TC`<br/>

`T`<br/>

#### Parameters

`specification` [IConsumerMessageSpecification\<TC, T\>](../masstransit-configuration/iconsumermessagespecification-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **AddPipeSpecification(IPipeSpecification\<ConsumerConsumeContext\<TConsumer, TMessage\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<ConsumerConsumeContext<TConsumer, TMessage>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **AddPipeSpecification(IPipeSpecification\<ConsumeContext\<TMessage\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<ConsumeContext<TMessage>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **Build(IFilter\<ConsumerConsumeContext\<TConsumer, TMessage\>\>)**

```csharp
public IPipe<ConsumerConsumeContext<TConsumer, TMessage>> Build(IFilter<ConsumerConsumeContext<TConsumer, TMessage>> consumeFilter)
```

#### Parameters

`consumeFilter` [IFilter\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1)<br/>

#### Returns

[IPipe\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

### **BuildMessagePipe(Action\<IPipeConfigurator\<ConsumeContext\<TMessage\>\>\>)**

```csharp
public IPipe<ConsumeContext<TMessage>> BuildMessagePipe(Action<IPipeConfigurator<ConsumeContext<TMessage>>> configure)
```

#### Parameters

`configure` [Action\<IPipeConfigurator\<ConsumeContext\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

### **AddPipeSpecification(IPipeSpecification\<ConsumerConsumeContext\<TConsumer\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<ConsumerConsumeContext<TConsumer>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ConsumerConsumeContext\<TConsumer\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver)**

```csharp
public ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
```

#### Parameters

`observer` [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **Message(Action\<IConsumerMessageConfigurator\<TMessage\>\>)**

```csharp
public void Message(Action<IConsumerMessageConfigurator<TMessage>> configure)
```

#### Parameters

`configure` [Action\<IConsumerMessageConfigurator\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
