---

title: BusFactoryConfigurator

---

# BusFactoryConfigurator

Namespace: MassTransit.Configuration

```csharp
public abstract class BusFactoryConfigurator : IConsumePipeConfigurator, IPipeConfigurator<ConsumeContext>, IConsumerConfigurationObserverConnector, ISagaConfigurationObserverConnector, IHandlerConfigurationObserverConnector, IActivityConfigurationObserverConnector, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, ISendPipelineConfigurator, IPublishPipelineConfigurator, IEndpointConfigurationObserverConnector, IBusObserverConnector, IReceiveObserverConnector, IConsumeObserverConnector, ISendObserverConnector, IPublishObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BusFactoryConfigurator](../masstransit-configuration/busfactoryconfigurator)<br/>
Implements [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator), [IPipeConfigurator\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISagaConfigurationObserverConnector](../../masstransit-abstractions/masstransit/isagaconfigurationobserverconnector), [IHandlerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserverconnector), [IActivityConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iactivityconfigurationobserverconnector), [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver), [ISendPipelineConfigurator](../../masstransit-abstractions/masstransit/isendpipelineconfigurator), [IPublishPipelineConfigurator](../../masstransit-abstractions/masstransit/ipublishpipelineconfigurator), [IEndpointConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iendpointconfigurationobserverconnector), [IBusObserverConnector](../../masstransit-abstractions/masstransit/ibusobserverconnector), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector)

## Properties

### **MessageTopology**

```csharp
public IMessageTopologyConfigurator MessageTopology { get; }
```

#### Property Value

[IMessageTopologyConfigurator](../../masstransit-abstractions/masstransit-configuration/imessagetopologyconfigurator)<br/>

### **ConsumeTopology**

```csharp
public IConsumeTopologyConfigurator ConsumeTopology { get; }
```

#### Property Value

[IConsumeTopologyConfigurator](../../masstransit-abstractions/masstransit/iconsumetopologyconfigurator)<br/>

### **SendTopology**

```csharp
public ISendTopologyConfigurator SendTopology { get; }
```

#### Property Value

[ISendTopologyConfigurator](../../masstransit-abstractions/masstransit/isendtopologyconfigurator)<br/>

### **PublishTopology**

```csharp
public IPublishTopologyConfigurator PublishTopology { get; }
```

#### Property Value

[IPublishTopologyConfigurator](../../masstransit-abstractions/masstransit/ipublishtopologyconfigurator)<br/>

### **DeployTopologyOnly**

```csharp
public bool DeployTopologyOnly { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **DeployPublishTopology**

```csharp
public bool DeployPublishTopology { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **PrefetchCount**

```csharp
public int PrefetchCount { set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **DefaultContentType**

```csharp
public ContentType DefaultContentType { set; }
```

#### Property Value

ContentType<br/>

### **SerializerContentType**

```csharp
public ContentType SerializerContentType { set; }
```

#### Property Value

ContentType<br/>

### **AutoStart**

```csharp
public bool AutoStart { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **ConnectBusObserver(IBusObserver)**

```csharp
public ConnectHandle ConnectBusObserver(IBusObserver observer)
```

#### Parameters

`observer` [IBusObserver](../../masstransit-abstractions/masstransit/ibusobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectConsumeObserver(IConsumeObserver)**

```csharp
public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
```

#### Parameters

`observer` [IConsumeObserver](../../masstransit-abstractions/masstransit/iconsumeobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **AddPipeSpecification(IPipeSpecification\<ConsumeContext\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ConsumeContext\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **AddPrePipeSpecification(IPipeSpecification\<ConsumeContext\>)**

```csharp
public void AddPrePipeSpecification(IPipeSpecification<ConsumeContext> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ConsumeContext\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **AddPipeSpecification\<T\>(IPipeSpecification\<ConsumeContext\<T\>\>)**

```csharp
public void AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
```

#### Type Parameters

`T`<br/>

#### Parameters

`specification` [IPipeSpecification\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

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

### **ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver)**

```csharp
public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
```

#### Parameters

`observer` [IEndpointConfigurationObserver](../../masstransit-abstractions/masstransit/iendpointconfigurationobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectPublishObserver(IPublishObserver)**

```csharp
public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
```

#### Parameters

`observer` [IPublishObserver](../../masstransit-abstractions/masstransit/ipublishobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConfigurePublish(Action\<IPublishPipeConfigurator\>)**

```csharp
public void ConfigurePublish(Action<IPublishPipeConfigurator> callback)
```

#### Parameters

`callback` [Action\<IPublishPipeConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConnectReceiveObserver(IReceiveObserver)**

```csharp
public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
```

#### Parameters

`observer` [IReceiveObserver](../../masstransit-abstractions/masstransit/ireceiveobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectSendObserver(ISendObserver)**

```csharp
public ConnectHandle ConnectSendObserver(ISendObserver observer)
```

#### Parameters

`observer` [ISendObserver](../../masstransit-abstractions/masstransit/isendobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConfigureSend(Action\<ISendPipeConfigurator\>)**

```csharp
public void ConfigureSend(Action<ISendPipeConfigurator> callback)
```

#### Parameters

`callback` [Action\<ISendPipeConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Message\<T\>(Action\<IMessageTopologyConfigurator\<T\>\>)**

```csharp
public void Message<T>(Action<IMessageTopologyConfigurator<T>> configureTopology)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configureTopology` [Action\<IMessageTopologyConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Send\<T\>(Action\<IMessageSendTopologyConfigurator\<T\>\>)**

```csharp
public void Send<T>(Action<IMessageSendTopologyConfigurator<T>> configureTopology)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configureTopology` [Action\<IMessageSendTopologyConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Publish\<T\>(Action\<IMessagePublishTopologyConfigurator\<T\>\>)**

```csharp
public void Publish<T>(Action<IMessagePublishTopologyConfigurator<T>> configureTopology)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configureTopology` [Action\<IMessagePublishTopologyConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **AddSerializer(ISerializerFactory, Boolean)**

```csharp
public void AddSerializer(ISerializerFactory factory, bool isSerializer)
```

#### Parameters

`factory` [ISerializerFactory](../../masstransit-abstractions/masstransit/iserializerfactory)<br/>

`isSerializer` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **AddDeserializer(ISerializerFactory, Boolean)**

```csharp
public void AddDeserializer(ISerializerFactory factory, bool isDefault)
```

#### Parameters

`factory` [ISerializerFactory](../../masstransit-abstractions/masstransit/iserializerfactory)<br/>

`isDefault` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ClearSerialization()**

```csharp
public void ClearSerialization()
```
