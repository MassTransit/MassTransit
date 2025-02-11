---

title: DelayedRedeliveryConfigurationObserver

---

# DelayedRedeliveryConfigurationObserver

Namespace: MassTransit.Configuration

```csharp
public class DelayedRedeliveryConfigurationObserver : ScheduledRedeliveryConfigurationObserver, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, IMessageConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IMessageConfigurationObserver\>](../../masstransit-abstractions/masstransit-util/connectable-1) → [ConfigurationObserver](../masstransit-configuration/configurationobserver) → [ScheduledRedeliveryConfigurationObserver](../masstransit-configuration/scheduledredeliveryconfigurationobserver) → [DelayedRedeliveryConfigurationObserver](../masstransit-configuration/delayedredeliveryconfigurationobserver)<br/>
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

### **DelayedRedeliveryConfigurationObserver(IConsumePipeConfigurator, Action\<IRedeliveryConfigurator\>)**

```csharp
public DelayedRedeliveryConfigurationObserver(IConsumePipeConfigurator configurator, Action<IRedeliveryConfigurator> configure)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`configure` [Action\<IRedeliveryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

## Methods

### **AddRedeliveryPipeSpecification\<TMessage\>(IConsumePipeConfigurator)**

```csharp
protected IRedeliveryPipeSpecification AddRedeliveryPipeSpecification<TMessage>(IConsumePipeConfigurator configurator)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

#### Returns

[IRedeliveryPipeSpecification](../masstransit-configuration/iredeliverypipespecification)<br/>

### **Method7()**

```csharp
public void Method7()
```

### **Method8()**

```csharp
public void Method8()
```

### **Method9()**

```csharp
public void Method9()
```
