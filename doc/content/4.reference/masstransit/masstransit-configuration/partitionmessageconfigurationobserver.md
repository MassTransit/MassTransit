---

title: PartitionMessageConfigurationObserver

---

# PartitionMessageConfigurationObserver

Namespace: MassTransit.Configuration

```csharp
public class PartitionMessageConfigurationObserver : ConfigurationObserver, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, IMessageConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IMessageConfigurationObserver\>](../../masstransit-abstractions/masstransit-util/connectable-1) → [ConfigurationObserver](../masstransit-configuration/configurationobserver) → [PartitionMessageConfigurationObserver](../masstransit-configuration/partitionmessageconfigurationobserver)<br/>
Implements [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver), [IMessageConfigurationObserver](../../masstransit-abstractions/masstransit/imessageconfigurationobserver)

## Properties

### **Connected**

```csharp
public IMessageConfigurationObserver[] Connected { get; }
```

#### Property Value

[IMessageConfigurationObserver[]](../../masstransit-abstractions/masstransit/imessageconfigurationobserver)<br/>

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **PartitionMessageConfigurationObserver(IConsumePipeConfigurator, IPartitioner)**

```csharp
public PartitionMessageConfigurationObserver(IConsumePipeConfigurator configurator, IPartitioner partitioner)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`partitioner` [IPartitioner](../masstransit/ipartitioner)<br/>

## Methods

### **MessageConfigured\<TMessage\>(IConsumePipeConfigurator)**

```csharp
public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

### **BatchConsumerConfigured\<TConsumer, TMessage\>(IConsumerMessageConfigurator\<TConsumer, Batch\<TMessage\>\>)**

```csharp
public void BatchConsumerConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, Batch<TMessage>> configurator)
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

#### Parameters

`configurator` [IConsumerMessageConfigurator\<TConsumer, Batch\<TMessage\>\>](../../masstransit-abstractions/masstransit/iconsumermessageconfigurator-2)<br/>

### **Method4()**

```csharp
public void Method4()
```

### **Method5()**

```csharp
public void Method5()
```

### **Method6()**

```csharp
public void Method6()
```
