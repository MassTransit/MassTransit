---

title: ReceiveEndpointConfiguration

---

# ReceiveEndpointConfiguration

Namespace: MassTransit.Configuration

```csharp
public abstract class ReceiveEndpointConfiguration : EndpointConfiguration, IEndpointConfiguration, IConsumePipeConfigurator, IPipeConfigurator<ConsumeContext>, IConsumerConfigurationObserverConnector, ISagaConfigurationObserverConnector, IHandlerConfigurationObserverConnector, IActivityConfigurationObserverConnector, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, ISendPipelineConfigurator, IPublishPipelineConfigurator, IReceivePipelineConfigurator, ISpecification, IReceiveEndpointConfiguration, IReceiveEndpointObserverConnector, IReceiveEndpointDependentConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EndpointConfiguration](../masstransit-configuration/endpointconfiguration) → [ReceiveEndpointConfiguration](../masstransit-configuration/receiveendpointconfiguration)<br/>
Implements [IEndpointConfiguration](../masstransit-configuration/iendpointconfiguration), [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator), [IPipeConfigurator\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISagaConfigurationObserverConnector](../../masstransit-abstractions/masstransit/isagaconfigurationobserverconnector), [IHandlerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserverconnector), [IActivityConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iactivityconfigurationobserverconnector), [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver), [ISendPipelineConfigurator](../../masstransit-abstractions/masstransit/isendpipelineconfigurator), [IPublishPipelineConfigurator](../../masstransit-abstractions/masstransit/ipublishpipelineconfigurator), [IReceivePipelineConfigurator](../../masstransit-abstractions/masstransit/ireceivepipelineconfigurator), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IReceiveEndpointConfiguration](../masstransit-configuration/ireceiveendpointconfiguration), [IReceiveEndpointObserverConnector](../../masstransit-abstractions/masstransit/ireceiveendpointobserverconnector), [IReceiveEndpointDependentConnector](../../masstransit-abstractions/masstransit/ireceiveendpointdependentconnector)

## Properties

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

### **HostAddress**

```csharp
public abstract Uri HostAddress { get; }
```

#### Property Value

Uri<br/>

### **InputAddress**

```csharp
public abstract Uri InputAddress { get; }
```

#### Property Value

Uri<br/>

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

## Methods

### **ConnectReceiveEndpointObserver(IReceiveEndpointObserver)**

```csharp
public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
```

#### Parameters

`observer` [IReceiveEndpointObserver](../../masstransit-abstractions/masstransit/ireceiveendpointobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **AddDependent(IReceiveEndpointDependent)**

```csharp
public void AddDependent(IReceiveEndpointDependent dependent)
```

#### Parameters

`dependent` [IReceiveEndpointDependent](../../masstransit-abstractions/masstransit-transports/ireceiveendpointdependent)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **CreateReceivePipe()**

```csharp
public IReceivePipe CreateReceivePipe()
```

#### Returns

[IReceivePipe](../../masstransit-abstractions/masstransit-transports/ireceivepipe)<br/>

### **CreateReceiveEndpointContext()**

```csharp
public abstract ReceiveEndpointContext CreateReceiveEndpointContext()
```

#### Returns

[ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>

### **ConfigureMessageTopology\<T\>(Boolean)**

```csharp
public void ConfigureMessageTopology<T>(bool enabled)
```

#### Type Parameters

`T`<br/>

#### Parameters

`enabled` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ConfigureMessageTopology(Type, Boolean)**

```csharp
public void ConfigureMessageTopology(Type messageType, bool enabled)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`enabled` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **AddDependency(IReceiveEndpointDependency)**

```csharp
public void AddDependency(IReceiveEndpointDependency dependency)
```

#### Parameters

`dependency` [IReceiveEndpointDependency](../../masstransit-abstractions/masstransit-transports/ireceiveendpointdependency)<br/>

### **ApplySpecifications(IReceiveEndpointBuilder)**

```csharp
protected void ApplySpecifications(IReceiveEndpointBuilder builder)
```

#### Parameters

`builder` [IReceiveEndpointBuilder](../../masstransit-abstractions/masstransit-configuration/ireceiveendpointbuilder)<br/>

### **AddEndpointSpecification(IReceiveEndpointSpecification)**

```csharp
public void AddEndpointSpecification(IReceiveEndpointSpecification specification)
```

#### Parameters

`specification` [IReceiveEndpointSpecification](../../masstransit-abstractions/masstransit/ireceiveendpointspecification)<br/>

### **CreateConsumePipe()**

```csharp
protected IConsumePipe CreateConsumePipe()
```

#### Returns

[IConsumePipe](../../masstransit-abstractions/masstransit-transports/iconsumepipe)<br/>

### **Changed(String)**

```csharp
protected void Changed(string key)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **IsAlreadyConfigured()**

```csharp
protected bool IsAlreadyConfigured()
```

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
