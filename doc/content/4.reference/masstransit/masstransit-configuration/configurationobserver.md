---

title: ConfigurationObserver

---

# ConfigurationObserver

Namespace: MassTransit.Configuration

Combines the separate configuration observers into a single observer that is for each message type, called once, to configure each
 message pipeline only once. Only outputs the individual message events for configuring the pipeline.

```csharp
public class ConfigurationObserver : Connectable<IMessageConfigurationObserver>, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IMessageConfigurationObserver\>](../../masstransit-abstractions/masstransit-util/connectable-1) → [ConfigurationObserver](../masstransit-configuration/configurationobserver)<br/>
Implements [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver)

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

## Methods

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

### **StateMachineSagaConfigured\<TInstance\>(ISagaConfigurator\<TInstance\>, SagaStateMachine\<TInstance\>)**

```csharp
public void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
```

#### Type Parameters

`TInstance`<br/>

#### Parameters

`configurator` [ISagaConfigurator\<TInstance\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

`stateMachine` [SagaStateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/sagastatemachine-1)<br/>

### **BatchConsumerConfigured\<TConsumer, TMessage\>(IConsumerMessageConfigurator\<TConsumer, Batch\<TMessage\>\>)**

```csharp
public void BatchConsumerConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, Batch<TMessage>> configurator)
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

#### Parameters

`configurator` [IConsumerMessageConfigurator\<TConsumer, Batch\<TMessage\>\>](../../masstransit-abstractions/masstransit/iconsumermessageconfigurator-2)<br/>
