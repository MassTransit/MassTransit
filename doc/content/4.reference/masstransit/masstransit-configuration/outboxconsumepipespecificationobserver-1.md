---

title: OutboxConsumePipeSpecificationObserver<TContext>

---

# OutboxConsumePipeSpecificationObserver\<TContext\>

Namespace: MassTransit.Configuration

```csharp
public class OutboxConsumePipeSpecificationObserver<TContext> : IConsumerConfigurationObserver, ISagaConfigurationObserver, IActivityConfigurationObserver, IOutboxOptionsConfigurator
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [OutboxConsumePipeSpecificationObserver\<TContext\>](../masstransit-configuration/outboxconsumepipespecificationobserver-1)<br/>
Implements [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver), [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver), [IOutboxOptionsConfigurator](../masstransit/ioutboxoptionsconfigurator)

## Properties

### **MessageDeliveryLimit**

```csharp
public int MessageDeliveryLimit { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **MessageDeliveryTimeout**

```csharp
public TimeSpan MessageDeliveryTimeout { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Constructors

### **OutboxConsumePipeSpecificationObserver(IReceiveEndpointConfigurator, IRegistrationContext)**

```csharp
public OutboxConsumePipeSpecificationObserver(IReceiveEndpointConfigurator configurator, IRegistrationContext context)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

### **OutboxConsumePipeSpecificationObserver(IReceiveEndpointConfigurator, IServiceProvider, ISetScopedConsumeContext)**

```csharp
public OutboxConsumePipeSpecificationObserver(IReceiveEndpointConfigurator configurator, IServiceProvider serviceProvider, ISetScopedConsumeContext setter)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`serviceProvider` IServiceProvider<br/>

`setter` [ISetScopedConsumeContext](../masstransit/isetscopedconsumecontext)<br/>

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

### **ConsumerConfigured\<TConsumer\>(IConsumerConfigurator\<TConsumer\>)**

```csharp
public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`configurator` [IConsumerConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1)<br/>

### **ConsumerMessageConfigured\<TConsumer, TMessage\>(IConsumerMessageConfigurator\<TConsumer, TMessage\>)**

```csharp
public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

#### Parameters

`configurator` [IConsumerMessageConfigurator\<TConsumer, TMessage\>](../../masstransit-abstractions/masstransit/iconsumermessageconfigurator-2)<br/>

### **SagaConfigured\<TSaga\>(ISagaConfigurator\<TSaga\>)**

```csharp
public void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`configurator` [ISagaConfigurator\<TSaga\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

### **StateMachineSagaConfigured\<TInstance\>(ISagaConfigurator\<TInstance\>, SagaStateMachine\<TInstance\>)**

```csharp
public void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
```

#### Type Parameters

`TInstance`<br/>

#### Parameters

`configurator` [ISagaConfigurator\<TInstance\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

`stateMachine` [SagaStateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/sagastatemachine-1)<br/>

### **SagaMessageConfigured\<TSaga, TMessage\>(ISagaMessageConfigurator\<TSaga, TMessage\>)**

```csharp
public void SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`configurator` [ISagaMessageConfigurator\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/isagamessageconfigurator-2)<br/>
