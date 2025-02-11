---

title: BatchConsumerMessageSpecification<TConsumer, TMessage>

---

# BatchConsumerMessageSpecification\<TConsumer, TMessage\>

Namespace: MassTransit.Configuration

Configures the pipe for a consumer/message combination within a consumer configuration
 block. Does not add any handlers to the message pipe standalone, everything is within
 the consumer pipe segment.

```csharp
public class BatchConsumerMessageSpecification<TConsumer, TMessage> : IConsumerMessageSpecification<TConsumer, Batch<TMessage>>, IConsumerMessageSpecification<TConsumer>, IPipeConfigurator<ConsumerConsumeContext<TConsumer>>, IConsumerConfigurationObserverConnector, ISpecification, IConsumerMessageConfigurator<TConsumer, Batch<TMessage>>, IPipeConfigurator<ConsumerConsumeContext<TConsumer, Batch<TMessage>>>, IConsumerMessageConfigurator<Batch<TMessage>>, IPipeConfigurator<ConsumeContext<Batch<TMessage>>>, IConsumerMessageConfigurator<TConsumer, TMessage>, IPipeConfigurator<ConsumerConsumeContext<TConsumer, TMessage>>
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BatchConsumerMessageSpecification\<TConsumer, TMessage\>](../masstransit-configuration/batchconsumermessagespecification-2)<br/>
Implements [IConsumerMessageSpecification\<TConsumer, Batch\<TMessage\>\>](../masstransit-configuration/iconsumermessagespecification-2), [IConsumerMessageSpecification\<TConsumer\>](../masstransit-configuration/iconsumermessagespecification-1), [IPipeConfigurator\<ConsumerConsumeContext\<TConsumer\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IConsumerMessageConfigurator\<TConsumer, Batch\<TMessage\>\>](../../masstransit-abstractions/masstransit/iconsumermessageconfigurator-2), [IPipeConfigurator\<ConsumerConsumeContext\<TConsumer, Batch\<TMessage\>\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerMessageConfigurator\<Batch\<TMessage\>\>](../../masstransit-abstractions/masstransit/iconsumermessageconfigurator-1), [IPipeConfigurator\<ConsumeContext\<Batch\<TMessage\>\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerMessageConfigurator\<TConsumer, TMessage\>](../../masstransit-abstractions/masstransit/iconsumermessageconfigurator-2), [IPipeConfigurator\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)

## Properties

### **MessageType**

```csharp
public Type MessageType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

## Constructors

### **BatchConsumerMessageSpecification()**

```csharp
public BatchConsumerMessageSpecification()
```

## Methods

### **AddPipeSpecification(IPipeSpecification\<ConsumerConsumeContext\<TConsumer, TMessage\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<ConsumerConsumeContext<TConsumer, TMessage>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **Message(Action\<IConsumerMessageConfigurator\<TMessage\>\>)**

```csharp
public void Message(Action<IConsumerMessageConfigurator<TMessage>> configure)
```

#### Parameters

`configure` [Action\<IConsumerMessageConfigurator\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

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

### **AddPipeSpecification(IPipeSpecification\<ConsumerConsumeContext\<TConsumer, Batch\<TMessage\>\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<ConsumerConsumeContext<TConsumer, Batch<TMessage>>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ConsumerConsumeContext\<TConsumer, Batch\<TMessage\>\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **AddPipeSpecification(IPipeSpecification\<ConsumeContext\<Batch\<TMessage\>\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<ConsumeContext<Batch<TMessage>>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ConsumeContext\<Batch\<TMessage\>\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **Build(IFilter\<ConsumerConsumeContext\<TConsumer, Batch\<TMessage\>\>\>)**

```csharp
public IPipe<ConsumerConsumeContext<TConsumer, Batch<TMessage>>> Build(IFilter<ConsumerConsumeContext<TConsumer, Batch<TMessage>>> consumeFilter)
```

#### Parameters

`consumeFilter` [IFilter\<ConsumerConsumeContext\<TConsumer, Batch\<TMessage\>\>\>](../../masstransit-abstractions/masstransit/ifilter-1)<br/>

#### Returns

[IPipe\<ConsumerConsumeContext\<TConsumer, Batch\<TMessage\>\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

### **BuildMessagePipe(Action\<IPipeConfigurator\<ConsumeContext\<Batch\<TMessage\>\>\>\>)**

```csharp
public IPipe<ConsumeContext<Batch<TMessage>>> BuildMessagePipe(Action<IPipeConfigurator<ConsumeContext<Batch<TMessage>>>> configure)
```

#### Parameters

`configure` [Action\<IPipeConfigurator\<ConsumeContext\<Batch\<TMessage\>\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IPipe\<ConsumeContext\<Batch\<TMessage\>\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

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

### **Message(Action\<IConsumerMessageConfigurator\<Batch\<TMessage\>\>\>)**

```csharp
public void Message(Action<IConsumerMessageConfigurator<Batch<TMessage>>> configure)
```

#### Parameters

`configure` [Action\<IConsumerMessageConfigurator\<Batch\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
