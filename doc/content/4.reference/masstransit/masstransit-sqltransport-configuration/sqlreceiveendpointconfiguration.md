---

title: SqlReceiveEndpointConfiguration

---

# SqlReceiveEndpointConfiguration

Namespace: MassTransit.SqlTransport.Configuration

```csharp
public class SqlReceiveEndpointConfiguration : ReceiveEndpointConfiguration, IEndpointConfiguration, IConsumePipeConfigurator, IPipeConfigurator<ConsumeContext>, IConsumerConfigurationObserverConnector, ISagaConfigurationObserverConnector, IHandlerConfigurationObserverConnector, IActivityConfigurationObserverConnector, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, ISendPipelineConfigurator, IPublishPipelineConfigurator, IReceivePipelineConfigurator, ISpecification, IReceiveEndpointConfiguration, IReceiveEndpointObserverConnector, IReceiveEndpointDependentConnector, ISqlReceiveEndpointConfiguration, ISqlEndpointConfiguration, ISqlReceiveEndpointConfigurator, IReceiveEndpointConfigurator, IEndpointConfigurator, IReceiveEndpointDependencyConnector, ISqlQueueEndpointConfigurator, ISqlQueueConfigurator
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EndpointConfiguration](../masstransit-configuration/endpointconfiguration) → [ReceiveEndpointConfiguration](../masstransit-configuration/receiveendpointconfiguration) → [SqlReceiveEndpointConfiguration](../masstransit-sqltransport-configuration/sqlreceiveendpointconfiguration)<br/>
Implements [IEndpointConfiguration](../masstransit-configuration/iendpointconfiguration), [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator), [IPipeConfigurator\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISagaConfigurationObserverConnector](../../masstransit-abstractions/masstransit/isagaconfigurationobserverconnector), [IHandlerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserverconnector), [IActivityConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iactivityconfigurationobserverconnector), [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver), [ISendPipelineConfigurator](../../masstransit-abstractions/masstransit/isendpipelineconfigurator), [IPublishPipelineConfigurator](../../masstransit-abstractions/masstransit/ipublishpipelineconfigurator), [IReceivePipelineConfigurator](../../masstransit-abstractions/masstransit/ireceivepipelineconfigurator), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IReceiveEndpointConfiguration](../masstransit-configuration/ireceiveendpointconfiguration), [IReceiveEndpointObserverConnector](../../masstransit-abstractions/masstransit/ireceiveendpointobserverconnector), [IReceiveEndpointDependentConnector](../../masstransit-abstractions/masstransit/ireceiveendpointdependentconnector), [ISqlReceiveEndpointConfiguration](../masstransit-sqltransport-configuration/isqlreceiveendpointconfiguration), [ISqlEndpointConfiguration](../masstransit-sqltransport-configuration/isqlendpointconfiguration), [ISqlReceiveEndpointConfigurator](../masstransit/isqlreceiveendpointconfigurator), [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator), [IEndpointConfigurator](../../masstransit-abstractions/masstransit/iendpointconfigurator), [IReceiveEndpointDependencyConnector](../../masstransit-abstractions/masstransit/ireceiveendpointdependencyconnector), [ISqlQueueEndpointConfigurator](../masstransit/isqlqueueendpointconfigurator), [ISqlQueueConfigurator](../masstransit/isqlqueueconfigurator)

## Properties

### **Settings**

```csharp
public ReceiveSettings Settings { get; }
```

#### Property Value

[ReceiveSettings](../masstransit-sqltransport/receivesettings)<br/>

### **HostAddress**

```csharp
public Uri HostAddress { get; }
```

#### Property Value

Uri<br/>

### **InputAddress**

```csharp
public Uri InputAddress { get; }
```

#### Property Value

Uri<br/>

### **AutoDeleteOnIdle**

```csharp
public Nullable<TimeSpan> AutoDeleteOnIdle { set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **PollingInterval**

```csharp
public TimeSpan PollingInterval { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **LockDuration**

```csharp
public TimeSpan LockDuration { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **MaxLockDuration**

```csharp
public TimeSpan MaxLockDuration { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **MaxDeliveryCount**

```csharp
public Nullable<int> MaxDeliveryCount { set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **PurgeOnStartup**

```csharp
public bool PurgeOnStartup { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **MaintenanceBatchSize**

```csharp
public int MaintenanceBatchSize { set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **DeadLetterExpiredMessages**

```csharp
public bool DeadLetterExpiredMessages { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **UnlockDelay**

```csharp
public Nullable<TimeSpan> UnlockDelay { set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConcurrentDeliveryLimit**

```csharp
public int ConcurrentDeliveryLimit { set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **EndpointObservers**

```csharp
public ReceiveEndpointObservable EndpointObservers { get; }
```

#### Property Value

[ReceiveEndpointObservable](../../masstransit-abstractions/masstransit-observables/receiveendpointobservable)<br/>

### **ReceiveObservers**

```csharp
public ReceiveObservable ReceiveObservers { get; }
```

#### Property Value

[ReceiveObservable](../../masstransit-abstractions/masstransit-observables/receiveobservable)<br/>

### **TransportObservers**

```csharp
public ReceiveTransportObservable TransportObservers { get; }
```

#### Property Value

[ReceiveTransportObservable](../../masstransit-abstractions/masstransit-observables/receivetransportobservable)<br/>

### **ConfigureConsumeTopology**

```csharp
public bool ConfigureConsumeTopology { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **PublishFaults**

```csharp
public bool PublishFaults { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ConsumePipe**

```csharp
public IConsumePipe ConsumePipe { get; }
```

#### Property Value

[IConsumePipe](../../masstransit-abstractions/masstransit-transports/iconsumepipe)<br/>

### **ReceiveEndpoint**

```csharp
public IReceiveEndpoint ReceiveEndpoint { get; protected set; }
```

#### Property Value

[IReceiveEndpoint](../../masstransit-abstractions/masstransit/ireceiveendpoint)<br/>

### **DependenciesReady**

```csharp
public Task DependenciesReady { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **DependentsCompleted**

```csharp
public Task DependentsCompleted { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **PrefetchCount**

```csharp
public int PrefetchCount { get; set; }
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

### **IsBusEndpoint**

```csharp
public bool IsBusEndpoint { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **AutoStart**

```csharp
public bool AutoStart { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Consume**

```csharp
public IConsumePipeConfiguration Consume { get; }
```

#### Property Value

[IConsumePipeConfiguration](../masstransit-configuration/iconsumepipeconfiguration)<br/>

### **Send**

```csharp
public ISendPipeConfiguration Send { get; }
```

#### Property Value

[ISendPipeConfiguration](../masstransit-configuration/isendpipeconfiguration)<br/>

### **Publish**

```csharp
public IPublishPipeConfiguration Publish { get; }
```

#### Property Value

[IPublishPipeConfiguration](../masstransit-configuration/ipublishpipeconfiguration)<br/>

### **Receive**

```csharp
public IReceivePipeConfiguration Receive { get; }
```

#### Property Value

[IReceivePipeConfiguration](../masstransit-configuration/ireceivepipeconfiguration)<br/>

### **Topology**

```csharp
public ITopologyConfiguration Topology { get; }
```

#### Property Value

[ITopologyConfiguration](../masstransit-configuration/itopologyconfiguration)<br/>

### **Serialization**

```csharp
public ISerializationConfiguration Serialization { get; }
```

#### Property Value

[ISerializationConfiguration](../masstransit-configuration/iserializationconfiguration)<br/>

### **Transport**

```csharp
public ITransportConfiguration Transport { get; }
```

#### Property Value

[ITransportConfiguration](../masstransit-configuration/itransportconfiguration)<br/>

## Constructors

### **SqlReceiveEndpointConfiguration(ISqlHostConfiguration, SqlReceiveSettings, ISqlEndpointConfiguration)**

```csharp
public SqlReceiveEndpointConfiguration(ISqlHostConfiguration hostConfiguration, SqlReceiveSettings settings, ISqlEndpointConfiguration endpointConfiguration)
```

#### Parameters

`hostConfiguration` [ISqlHostConfiguration](../masstransit-sqltransport-configuration/isqlhostconfiguration)<br/>

`settings` [SqlReceiveSettings](../masstransit-sqltransport-configuration/sqlreceivesettings)<br/>

`endpointConfiguration` [ISqlEndpointConfiguration](../masstransit-sqltransport-configuration/isqlendpointconfiguration)<br/>

## Methods

### **CreateReceiveEndpointContext()**

```csharp
public ReceiveEndpointContext CreateReceiveEndpointContext()
```

#### Returns

[ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>

### **Build(IHost)**

```csharp
public void Build(IHost host)
```

#### Parameters

`host` [IHost](../masstransit-transports/ihost)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Subscribe(String, Action\<ISqlTopicSubscriptionConfigurator\>)**

```csharp
public void Subscribe(string topicName, Action<ISqlTopicSubscriptionConfigurator> callback)
```

#### Parameters

`topicName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`callback` [Action\<ISqlTopicSubscriptionConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Subscribe\<T\>(Action\<ISqlTopicSubscriptionConfigurator\>)**

```csharp
public void Subscribe<T>(Action<ISqlTopicSubscriptionConfigurator> callback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`callback` [Action\<ISqlTopicSubscriptionConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **SetReceiveMode(SqlReceiveMode, Nullable\<Int32\>)**

```csharp
public void SetReceiveMode(SqlReceiveMode mode, Nullable<int> concurrentDeliveryLimit)
```

#### Parameters

`mode` [SqlReceiveMode](../masstransit/sqlreceivemode)<br/>

`concurrentDeliveryLimit` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConfigureClient(Action\<IPipeConfigurator\<ClientContext\>\>)**

```csharp
public void ConfigureClient(Action<IPipeConfigurator<ClientContext>> configure)
```

#### Parameters

`configure` [Action\<IPipeConfigurator\<ClientContext\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **IsAlreadyConfigured()**

```csharp
protected bool IsAlreadyConfigured()
```

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
