---

title: InMemoryHostConfiguration

---

# InMemoryHostConfiguration

Namespace: MassTransit.InMemoryTransport.Configuration

```csharp
public class InMemoryHostConfiguration : BaseHostConfiguration<IInMemoryReceiveEndpointConfiguration, IInMemoryReceiveEndpointConfigurator>, IHostConfiguration, IEndpointConfigurationObserverConnector, IReceiveObserverConnector, IConsumeObserverConnector, IPublishObserverConnector, ISendObserverConnector, ISpecification, IReceiveConfigurator<IInMemoryReceiveEndpointConfigurator>, IReceiveConfigurator, IInMemoryHostConfiguration, IInMemoryHostConfigurator
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseHostConfiguration\<IInMemoryReceiveEndpointConfiguration, IInMemoryReceiveEndpointConfigurator\>](../masstransit-configuration/basehostconfiguration-2) → [InMemoryHostConfiguration](../masstransit-inmemorytransport-configuration/inmemoryhostconfiguration)<br/>
Implements [IHostConfiguration](../masstransit-configuration/ihostconfiguration), [IEndpointConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iendpointconfigurationobserverconnector), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IReceiveConfigurator\<IInMemoryReceiveEndpointConfigurator\>](../../masstransit-abstractions/masstransit/ireceiveconfigurator-1), [IReceiveConfigurator](../../masstransit-abstractions/masstransit/ireceiveconfigurator), [IInMemoryHostConfiguration](../masstransit-inmemorytransport-configuration/iinmemoryhostconfiguration), [IInMemoryHostConfigurator](../masstransit/iinmemoryhostconfigurator)

## Properties

### **HostAddress**

```csharp
public Uri HostAddress { get; }
```

#### Property Value

Uri<br/>

### **Topology**

```csharp
public IBusTopology Topology { get; }
```

#### Property Value

[IBusTopology](../../masstransit-abstractions/masstransit/ibustopology)<br/>

### **ReceiveTransportRetryPolicy**

```csharp
public IRetryPolicy ReceiveTransportRetryPolicy { get; }
```

#### Property Value

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **BaseAddress**

```csharp
public Uri BaseAddress { set; }
```

#### Property Value

Uri<br/>

### **BusConfiguration**

```csharp
public IBusConfiguration BusConfiguration { get; }
```

#### Property Value

[IBusConfiguration](../masstransit-configuration/ibusconfiguration)<br/>

### **DeployTopologyOnly**

```csharp
public bool DeployTopologyOnly { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **DeployPublishTopology**

```csharp
public bool DeployPublishTopology { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **SendObservers**

```csharp
public ISendObserver SendObservers { get; }
```

#### Property Value

[ISendObserver](../../masstransit-abstractions/masstransit/isendobserver)<br/>

### **LogContext**

```csharp
public ILogContext LogContext { get; set; }
```

#### Property Value

[ILogContext](../masstransit-logging/ilogcontext)<br/>

### **ReceiveLogContext**

```csharp
public ILogContext ReceiveLogContext { get; }
```

#### Property Value

[ILogContext](../masstransit-logging/ilogcontext)<br/>

### **SendLogContext**

```csharp
public ILogContext SendLogContext { get; }
```

#### Property Value

[ILogContext](../masstransit-logging/ilogcontext)<br/>

### **SendTransportRetryPolicy**

```csharp
public IRetryPolicy SendTransportRetryPolicy { get; }
```

#### Property Value

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **ConsumerStopTimeout**

```csharp
public Nullable<TimeSpan> ConsumerStopTimeout { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StopTimeout**

```csharp
public Nullable<TimeSpan> StopTimeout { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **InMemoryHostConfiguration(IInMemoryBusConfiguration, Uri, IInMemoryTopologyConfiguration)**

```csharp
public InMemoryHostConfiguration(IInMemoryBusConfiguration busConfiguration, Uri baseAddress, IInMemoryTopologyConfiguration topologyConfiguration)
```

#### Parameters

`busConfiguration` [IInMemoryBusConfiguration](../masstransit-inmemorytransport-configuration/iinmemorybusconfiguration)<br/>

`baseAddress` Uri<br/>

`topologyConfiguration` [IInMemoryTopologyConfiguration](../masstransit-inmemorytransport-configuration/iinmemorytopologyconfiguration)<br/>

## Methods

### **ApplyEndpointDefinition(IInMemoryReceiveEndpointConfigurator, IEndpointDefinition)**

```csharp
public void ApplyEndpointDefinition(IInMemoryReceiveEndpointConfigurator configurator, IEndpointDefinition definition)
```

#### Parameters

`configurator` [IInMemoryReceiveEndpointConfigurator](../masstransit/iinmemoryreceiveendpointconfigurator)<br/>

`definition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

### **CreateReceiveEndpointConfiguration(String, Action\<IInMemoryReceiveEndpointConfigurator\>)**

```csharp
public IInMemoryReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, Action<IInMemoryReceiveEndpointConfigurator> configure)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configure` [Action\<IInMemoryReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IInMemoryReceiveEndpointConfiguration](../masstransit-inmemorytransport-configuration/iinmemoryreceiveendpointconfiguration)<br/>

### **CreateReceiveEndpointConfiguration(String, IInMemoryEndpointConfiguration, Action\<IInMemoryReceiveEndpointConfigurator\>)**

```csharp
public IInMemoryReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, IInMemoryEndpointConfiguration endpointConfiguration, Action<IInMemoryReceiveEndpointConfigurator> configure)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`endpointConfiguration` [IInMemoryEndpointConfiguration](../masstransit-inmemorytransport-configuration/iinmemoryendpointconfiguration)<br/>

`configure` [Action\<IInMemoryReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IInMemoryReceiveEndpointConfiguration](../masstransit-inmemorytransport-configuration/iinmemoryreceiveendpointconfiguration)<br/>

### **ReceiveEndpoint(IEndpointDefinition, IEndpointNameFormatter, Action\<IInMemoryReceiveEndpointConfigurator\>)**

```csharp
public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<IInMemoryReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`definition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

`configureEndpoint` [Action\<IInMemoryReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ReceiveEndpoint(String, Action\<IInMemoryReceiveEndpointConfigurator\>)**

```csharp
public void ReceiveEndpoint(string queueName, Action<IInMemoryReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configureEndpoint` [Action\<IInMemoryReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **CreateReceiveEndpointConfiguration(String, Action\<IReceiveEndpointConfigurator\>)**

```csharp
public IReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, Action<IReceiveEndpointConfigurator> configure)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configure` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IReceiveEndpointConfiguration](../masstransit-configuration/ireceiveendpointconfiguration)<br/>

### **Build()**

```csharp
public IHost Build()
```

#### Returns

[IHost](../masstransit-transports/ihost)<br/>
