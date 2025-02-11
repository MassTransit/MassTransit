---

title: MessageScopeConfigurationObserver

---

# MessageScopeConfigurationObserver

Namespace: MassTransit.Configuration

```csharp
public class MessageScopeConfigurationObserver : ConfigurationObserver, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, IMessageConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IMessageConfigurationObserver\>](../../masstransit-abstractions/masstransit-util/connectable-1) → [ConfigurationObserver](../masstransit-configuration/configurationobserver) → [MessageScopeConfigurationObserver](../masstransit-configuration/messagescopeconfigurationobserver)<br/>
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

### **MessageScopeConfigurationObserver(IConsumePipeConfigurator, IServiceProvider)**

```csharp
public MessageScopeConfigurationObserver(IConsumePipeConfigurator receiveEndpointConfigurator, IServiceProvider serviceProvider)
```

#### Parameters

`receiveEndpointConfigurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`serviceProvider` IServiceProvider<br/>

### **MessageScopeConfigurationObserver(IConsumePipeConfigurator, IServiceProvider, ISetScopedConsumeContext)**

```csharp
public MessageScopeConfigurationObserver(IConsumePipeConfigurator receiveEndpointConfigurator, IServiceProvider serviceProvider, ISetScopedConsumeContext setScopedConsumeContext)
```

#### Parameters

`receiveEndpointConfigurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`serviceProvider` IServiceProvider<br/>

`setScopedConsumeContext` [ISetScopedConsumeContext](../masstransit/isetscopedconsumecontext)<br/>

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

### **ActivityConfigured\<TActivity, TArguments\>(IExecuteActivityConfigurator\<TActivity, TArguments\>, Uri)**

```csharp
public void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IExecuteActivityConfigurator\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityconfigurator-2)<br/>

`compensateAddress` Uri<br/>

### **ExecuteActivityConfigured\<TActivity, TArguments\>(IExecuteActivityConfigurator\<TActivity, TArguments\>)**

```csharp
public void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IExecuteActivityConfigurator\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityconfigurator-2)<br/>

### **CompensateActivityConfigured\<TActivity, TLog\>(ICompensateActivityConfigurator\<TActivity, TLog\>)**

```csharp
public void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`configurator` [ICompensateActivityConfigurator\<TActivity, TLog\>](../../masstransit-abstractions/masstransit/icompensateactivityconfigurator-2)<br/>

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
