---

title: BaseHostConfiguration<TConfiguration, TConfigurator>

---

# BaseHostConfiguration\<TConfiguration, TConfigurator\>

Namespace: MassTransit.Configuration

```csharp
public abstract class BaseHostConfiguration<TConfiguration, TConfigurator> : IHostConfiguration, IEndpointConfigurationObserverConnector, IReceiveObserverConnector, IConsumeObserverConnector, IPublishObserverConnector, ISendObserverConnector, ISpecification, IReceiveConfigurator<TConfigurator>, IReceiveConfigurator
```

#### Type Parameters

`TConfiguration`<br/>

`TConfigurator`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseHostConfiguration\<TConfiguration, TConfigurator\>](../masstransit-configuration/basehostconfiguration-2)<br/>
Implements [IHostConfiguration](../masstransit-configuration/ihostconfiguration), [IEndpointConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iendpointconfigurationobserverconnector), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IReceiveConfigurator\<TConfigurator\>](../../masstransit-abstractions/masstransit/ireceiveconfigurator-1), [IReceiveConfigurator](../../masstransit-abstractions/masstransit/ireceiveconfigurator)

## Properties

### **BusConfiguration**

```csharp
public IBusConfiguration BusConfiguration { get; }
```

#### Property Value

[IBusConfiguration](../masstransit-configuration/ibusconfiguration)<br/>

### **HostAddress**

```csharp
public abstract Uri HostAddress { get; }
```

#### Property Value

Uri<br/>

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
public ILogContext ReceiveLogContext { get; private set; }
```

#### Property Value

[ILogContext](../masstransit-logging/ilogcontext)<br/>

### **SendLogContext**

```csharp
public ILogContext SendLogContext { get; private set; }
```

#### Property Value

[ILogContext](../masstransit-logging/ilogcontext)<br/>

### **Topology**

```csharp
public abstract IBusTopology Topology { get; }
```

#### Property Value

[IBusTopology](../../masstransit-abstractions/masstransit/ibustopology)<br/>

### **ReceiveTransportRetryPolicy**

```csharp
public abstract IRetryPolicy ReceiveTransportRetryPolicy { get; }
```

#### Property Value

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

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

## Methods

### **ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver)**

```csharp
public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
```

#### Parameters

`observer` [IEndpointConfigurationObserver](../../masstransit-abstractions/masstransit/iendpointconfigurationobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectReceiveEndpointContext(ReceiveEndpointContext)**

```csharp
public ConnectHandle ConnectReceiveEndpointContext(ReceiveEndpointContext context)
```

#### Parameters

`context` [ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **CreateReceiveEndpointConfiguration(String, Action\<IReceiveEndpointConfigurator\>)**

```csharp
public abstract IReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, Action<IReceiveEndpointConfigurator> configure)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configure` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IReceiveEndpointConfiguration](../masstransit-configuration/ireceiveendpointconfiguration)<br/>

### **Build()**

```csharp
public abstract IHost Build()
```

#### Returns

[IHost](../masstransit-transports/ihost)<br/>

### **ConnectReceiveObserver(IReceiveObserver)**

```csharp
public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
```

#### Parameters

`observer` [IReceiveObserver](../../masstransit-abstractions/masstransit/ireceiveobserver)<br/>

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

### **ConnectPublishObserver(IPublishObserver)**

```csharp
public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
```

#### Parameters

`observer` [IPublishObserver](../../masstransit-abstractions/masstransit/ipublishobserver)<br/>

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

### **ReceiveEndpoint(String, Action\<IReceiveEndpointConfigurator\>)**

```csharp
public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configureEndpoint` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ReceiveEndpoint(IEndpointDefinition, IEndpointNameFormatter, Action\<IReceiveEndpointConfigurator\>)**

```csharp
public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<IReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`definition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

`configureEndpoint` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ReceiveEndpoint(IEndpointDefinition, IEndpointNameFormatter, Action\<TConfigurator\>)**

```csharp
public abstract void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<TConfigurator> configureEndpoint)
```

#### Parameters

`definition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

`configureEndpoint` [Action\<TConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ReceiveEndpoint(String, Action\<TConfigurator\>)**

```csharp
public abstract void ReceiveEndpoint(string queueName, Action<TConfigurator> configureEndpoint)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configureEndpoint` [Action\<TConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ApplyEndpointDefinition(IReceiveEndpointConfigurator, IEndpointDefinition)**

```csharp
protected void ApplyEndpointDefinition(IReceiveEndpointConfigurator configurator, IEndpointDefinition definition)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`definition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

### **GetConfiguredEndpoints()**

```csharp
protected IEnumerable<TConfiguration> GetConfiguredEndpoints()
```

#### Returns

[IEnumerable\<TConfiguration\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Add(TConfiguration)**

```csharp
protected void Add(TConfiguration configuration)
```

#### Parameters

`configuration` TConfiguration<br/>
