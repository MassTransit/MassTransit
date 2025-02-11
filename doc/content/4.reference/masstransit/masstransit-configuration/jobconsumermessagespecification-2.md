---

title: JobConsumerMessageSpecification<TConsumer, TJob>

---

# JobConsumerMessageSpecification\<TConsumer, TJob\>

Namespace: MassTransit.Configuration

```csharp
public class JobConsumerMessageSpecification<TConsumer, TJob> : IConsumerMessageSpecification<TConsumer, TJob>, IConsumerMessageSpecification<TConsumer>, IPipeConfigurator<ConsumerConsumeContext<TConsumer>>, IConsumerConfigurationObserverConnector, ISpecification, IConsumerMessageConfigurator<TConsumer, TJob>, IPipeConfigurator<ConsumerConsumeContext<TConsumer, TJob>>, IConsumerMessageConfigurator<TJob>, IPipeConfigurator<ConsumeContext<TJob>>
```

#### Type Parameters

`TConsumer`<br/>

`TJob`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobConsumerMessageSpecification\<TConsumer, TJob\>](../masstransit-configuration/jobconsumermessagespecification-2)<br/>
Implements [IConsumerMessageSpecification\<TConsumer, TJob\>](../masstransit-configuration/iconsumermessagespecification-2), [IConsumerMessageSpecification\<TConsumer\>](../masstransit-configuration/iconsumermessagespecification-1), [IPipeConfigurator\<ConsumerConsumeContext\<TConsumer\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IConsumerMessageConfigurator\<TConsumer, TJob\>](../../masstransit-abstractions/masstransit/iconsumermessageconfigurator-2), [IPipeConfigurator\<ConsumerConsumeContext\<TConsumer, TJob\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerMessageConfigurator\<TJob\>](../../masstransit-abstractions/masstransit/iconsumermessageconfigurator-1), [IPipeConfigurator\<ConsumeContext\<TJob\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)

## Properties

### **SubmitJobSpecification**

```csharp
public IConsumerSpecification<SubmitJobConsumer<TJob>> SubmitJobSpecification { get; }
```

#### Property Value

[IConsumerSpecification\<SubmitJobConsumer\<TJob\>\>](../masstransit-configuration/iconsumerspecification-1)<br/>

### **StartJobSpecification**

```csharp
public IConsumerSpecification<StartJobConsumer<TJob>> StartJobSpecification { get; }
```

#### Property Value

[IConsumerSpecification\<StartJobConsumer\<TJob\>\>](../masstransit-configuration/iconsumerspecification-1)<br/>

### **FinalizeJobSpecification**

```csharp
public IConsumerSpecification<FinalizeJobConsumer<TJob>> FinalizeJobSpecification { get; }
```

#### Property Value

[IConsumerSpecification\<FinalizeJobConsumer\<TJob\>\>](../masstransit-configuration/iconsumerspecification-1)<br/>

### **SuperviseJobSpecification**

```csharp
public IConsumerSpecification<SuperviseJobConsumer> SuperviseJobSpecification { get; }
```

#### Property Value

[IConsumerSpecification\<SuperviseJobConsumer\>](../masstransit-configuration/iconsumerspecification-1)<br/>

### **MessageType**

```csharp
public Type MessageType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

## Constructors

### **JobConsumerMessageSpecification()**

```csharp
public JobConsumerMessageSpecification()
```

## Methods

### **AddPipeSpecification(IPipeSpecification\<ConsumerConsumeContext\<TConsumer, TJob\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<ConsumerConsumeContext<TConsumer, TJob>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ConsumerConsumeContext\<TConsumer, TJob\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **Message(Action\<IConsumerMessageConfigurator\<TJob\>\>)**

```csharp
public void Message(Action<IConsumerMessageConfigurator<TJob>> configure)
```

#### Parameters

`configure` [Action\<IConsumerMessageConfigurator\<TJob\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

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

### **AddPipeSpecification(IPipeSpecification\<ConsumeContext\<TJob\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<ConsumeContext<TJob>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ConsumeContext\<TJob\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **Build(IFilter\<ConsumerConsumeContext\<TConsumer, TJob\>\>)**

```csharp
public IPipe<ConsumerConsumeContext<TConsumer, TJob>> Build(IFilter<ConsumerConsumeContext<TConsumer, TJob>> consumeFilter)
```

#### Parameters

`consumeFilter` [IFilter\<ConsumerConsumeContext\<TConsumer, TJob\>\>](../../masstransit-abstractions/masstransit/ifilter-1)<br/>

#### Returns

[IPipe\<ConsumerConsumeContext\<TConsumer, TJob\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

### **BuildMessagePipe(Action\<IPipeConfigurator\<ConsumeContext\<TJob\>\>\>)**

```csharp
public IPipe<ConsumeContext<TJob>> BuildMessagePipe(Action<IPipeConfigurator<ConsumeContext<TJob>>> configure)
```

#### Parameters

`configure` [Action\<IPipeConfigurator\<ConsumeContext\<TJob\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IPipe\<ConsumeContext\<TJob\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

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
