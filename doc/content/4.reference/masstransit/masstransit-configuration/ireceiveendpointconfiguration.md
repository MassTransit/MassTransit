---

title: IReceiveEndpointConfiguration

---

# IReceiveEndpointConfiguration

Namespace: MassTransit.Configuration

```csharp
public interface IReceiveEndpointConfiguration : IEndpointConfiguration, IConsumePipeConfigurator, IPipeConfigurator<ConsumeContext>, IConsumerConfigurationObserverConnector, ISagaConfigurationObserverConnector, IHandlerConfigurationObserverConnector, IActivityConfigurationObserverConnector, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, ISendPipelineConfigurator, IPublishPipelineConfigurator, IReceivePipelineConfigurator, ISpecification, IReceiveEndpointObserverConnector, IReceiveEndpointDependentConnector
```

Implements [IEndpointConfiguration](../masstransit-configuration/iendpointconfiguration), [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator), [IPipeConfigurator\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISagaConfigurationObserverConnector](../../masstransit-abstractions/masstransit/isagaconfigurationobserverconnector), [IHandlerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserverconnector), [IActivityConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iactivityconfigurationobserverconnector), [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver), [ISendPipelineConfigurator](../../masstransit-abstractions/masstransit/isendpipelineconfigurator), [IPublishPipelineConfigurator](../../masstransit-abstractions/masstransit/ipublishpipelineconfigurator), [IReceivePipelineConfigurator](../../masstransit-abstractions/masstransit/ireceivepipelineconfigurator), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IReceiveEndpointObserverConnector](../../masstransit-abstractions/masstransit/ireceiveendpointobserverconnector), [IReceiveEndpointDependentConnector](../../masstransit-abstractions/masstransit/ireceiveendpointdependentconnector)

## Properties

### **ConsumePipe**

```csharp
public abstract IConsumePipe ConsumePipe { get; }
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

### **ConfigureConsumeTopology**

```csharp
public abstract bool ConfigureConsumeTopology { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **PublishFaults**

```csharp
public abstract bool PublishFaults { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **PrefetchCount**

```csharp
public abstract int PrefetchCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ConcurrentMessageLimit**

```csharp
public abstract Nullable<int> ConcurrentMessageLimit { get; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **EndpointObservers**

```csharp
public abstract ReceiveEndpointObservable EndpointObservers { get; }
```

#### Property Value

[ReceiveEndpointObservable](../../masstransit-abstractions/masstransit-observables/receiveendpointobservable)<br/>

### **ReceiveObservers**

```csharp
public abstract ReceiveObservable ReceiveObservers { get; }
```

#### Property Value

[ReceiveObservable](../../masstransit-abstractions/masstransit-observables/receiveobservable)<br/>

### **TransportObservers**

```csharp
public abstract ReceiveTransportObservable TransportObservers { get; }
```

#### Property Value

[ReceiveTransportObservable](../../masstransit-abstractions/masstransit-observables/receivetransportobservable)<br/>

### **ReceiveEndpoint**

```csharp
public abstract IReceiveEndpoint ReceiveEndpoint { get; }
```

#### Property Value

[IReceiveEndpoint](../../masstransit-abstractions/masstransit/ireceiveendpoint)<br/>

### **DependenciesReady**

Completed once the receive endpoint dependencies are ready

```csharp
public abstract Task DependenciesReady { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **DependentsCompleted**

Completed once the receive endpoint dependents are completed

```csharp
public abstract Task DependentsCompleted { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

## Methods

### **CreateReceivePipe()**

Create the receive pipe, using the endpoint configuration

```csharp
IReceivePipe CreateReceivePipe()
```

#### Returns

[IReceivePipe](../../masstransit-abstractions/masstransit-transports/ireceivepipe)<br/>

### **CreateReceiveEndpointContext()**

```csharp
ReceiveEndpointContext CreateReceiveEndpointContext()
```

#### Returns

[ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>
