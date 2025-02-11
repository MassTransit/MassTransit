---

title: ConsumerSpecification<TConsumer>

---

# ConsumerSpecification\<TConsumer\>

Namespace: MassTransit.Configuration

```csharp
public class ConsumerSpecification<TConsumer> : OptionsSet, IOptionsSet, IConsumerSpecification<TConsumer>, IConsumerConfigurator<TConsumer>, IPipeConfigurator<ConsumerConsumeContext<TConsumer>>, IConsumerConfigurator, IConsumeConfigurator, IConsumerConfigurationObserverConnector, ISpecification
```

#### Type Parameters

`TConsumer`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [OptionsSet](../../masstransit-abstractions/masstransit-configuration/optionsset) → [ConsumerSpecification\<TConsumer\>](../masstransit-configuration/consumerspecification-1)<br/>
Implements [IOptionsSet](../../masstransit-abstractions/masstransit-configuration/ioptionsset), [IConsumerSpecification\<TConsumer\>](../masstransit-configuration/iconsumerspecification-1), [IConsumerConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1), [IPipeConfigurator\<ConsumerConsumeContext\<TConsumer\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurator](../../masstransit-abstractions/masstransit/iconsumerconfigurator), [IConsumeConfigurator](../../masstransit-abstractions/masstransit/iconsumeconfigurator), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **ConsumerSpecification(IEnumerable\<IConsumerMessageSpecification\<TConsumer\>\>)**

```csharp
public ConsumerSpecification(IEnumerable<IConsumerMessageSpecification<TConsumer>> messageSpecifications)
```

#### Parameters

`messageSpecifications` [IEnumerable\<IConsumerMessageSpecification\<TConsumer\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Methods

### **Message\<T\>(Action\<IConsumerMessageConfigurator\<T\>\>)**

```csharp
public void Message<T>(Action<IConsumerMessageConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configure` [Action\<IConsumerMessageConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConsumerMessage\<T\>(Action\<IConsumerMessageConfigurator\<TConsumer, T\>\>)**

```csharp
public void ConsumerMessage<T>(Action<IConsumerMessageConfigurator<TConsumer, T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configure` [Action\<IConsumerMessageConfigurator\<TConsumer, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **GetMessageSpecification\<T\>()**

```csharp
public IConsumerMessageSpecification<TConsumer, T> GetMessageSpecification<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IConsumerMessageSpecification\<TConsumer, T\>](../masstransit-configuration/iconsumermessagespecification-2)<br/>

### **ConfigureMessagePipe\<T\>(IPipeConfigurator\<ConsumeContext\<T\>\>)**

```csharp
public void ConfigureMessagePipe<T>(IPipeConfigurator<ConsumeContext<T>> pipeConfigurator)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipeConfigurator` [IPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

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
