---

title: ConsumePipeSpecification

---

# ConsumePipeSpecification

Namespace: MassTransit.Configuration

```csharp
public class ConsumePipeSpecification : IConsumePipeConfigurator, IPipeConfigurator<ConsumeContext>, IConsumerConfigurationObserverConnector, ISagaConfigurationObserverConnector, IHandlerConfigurationObserverConnector, IActivityConfigurationObserverConnector, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, IConsumePipeSpecification, IConsumePipeSpecificationObserverConnector, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumePipeSpecification](../masstransit-configuration/consumepipespecification)<br/>
Implements [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator), [IPipeConfigurator\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISagaConfigurationObserverConnector](../../masstransit-abstractions/masstransit/isagaconfigurationobserverconnector), [IHandlerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserverconnector), [IActivityConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iactivityconfigurationobserverconnector), [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver), [IConsumePipeSpecification](../../masstransit-abstractions/masstransit-configuration/iconsumepipespecification), [IConsumePipeSpecificationObserverConnector](../../masstransit-abstractions/masstransit-configuration/iconsumepipespecificationobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **AutoStart**

```csharp
public bool AutoStart { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **ConsumePipeSpecification()**

```csharp
public ConsumePipeSpecification()
```

## Methods

### **AddPipeSpecification(IPipeSpecification\<ConsumeContext\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ConsumeContext\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver)**

```csharp
public ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
```

#### Parameters

`observer` [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectSagaConfigurationObserver(ISagaConfigurationObserver)**

```csharp
public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
```

#### Parameters

`observer` [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectHandlerConfigurationObserver(IHandlerConfigurationObserver)**

```csharp
public ConnectHandle ConnectHandlerConfigurationObserver(IHandlerConfigurationObserver observer)
```

#### Parameters

`observer` [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectActivityConfigurationObserver(IActivityConfigurationObserver)**

```csharp
public ConnectHandle ConnectActivityConfigurationObserver(IActivityConfigurationObserver observer)
```

#### Parameters

`observer` [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **AddPipeSpecification\<T\>(IPipeSpecification\<ConsumeContext\<T\>\>)**

```csharp
public void AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
```

#### Type Parameters

`T`<br/>

#### Parameters

`specification` [IPipeSpecification\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **AddPrePipeSpecification(IPipeSpecification\<ConsumeContext\>)**

```csharp
public void AddPrePipeSpecification(IPipeSpecification<ConsumeContext> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ConsumeContext\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

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

### **HandlerConfigured\<TMessage\>(IHandlerConfigurator\<TMessage\>)**

```csharp
public void HandlerConfigured<TMessage>(IHandlerConfigurator<TMessage> configurator)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`configurator` [IHandlerConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/ihandlerconfigurator-1)<br/>

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

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **GetMessageSpecification\<T\>()**

```csharp
public IMessageConsumePipeSpecification<T> GetMessageSpecification<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IMessageConsumePipeSpecification\<T\>](../../masstransit-abstractions/masstransit-configuration/imessageconsumepipespecification-1)<br/>

### **ConnectConsumePipeSpecificationObserver(IConsumePipeSpecificationObserver)**

```csharp
public ConnectHandle ConnectConsumePipeSpecificationObserver(IConsumePipeSpecificationObserver observer)
```

#### Parameters

`observer` [IConsumePipeSpecificationObserver](../../masstransit-abstractions/masstransit-configuration/iconsumepipespecificationobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **BuildConsumePipe()**

```csharp
public IConsumePipe BuildConsumePipe()
```

#### Returns

[IConsumePipe](../../masstransit-abstractions/masstransit-transports/iconsumepipe)<br/>

### **CreateConsumePipeSpecification()**

```csharp
public IConsumePipeSpecification CreateConsumePipeSpecification()
```

#### Returns

[IConsumePipeSpecification](../../masstransit-abstractions/masstransit-configuration/iconsumepipespecification)<br/>
