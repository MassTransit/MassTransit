---

title: IHostConfiguration

---

# IHostConfiguration

Namespace: MassTransit.Configuration

```csharp
public interface IHostConfiguration : IEndpointConfigurationObserverConnector, IReceiveObserverConnector, IConsumeObserverConnector, IPublishObserverConnector, ISendObserverConnector, ISpecification
```

Implements [IEndpointConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iendpointconfigurationobserverconnector), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **BusConfiguration**

```csharp
public abstract IBusConfiguration BusConfiguration { get; }
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

If true, only the broker topology will be deployed

```csharp
public abstract bool DeployTopologyOnly { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **DeployPublishTopology**

If true, the publish topology will be deployed at startup

```csharp
public abstract bool DeployPublishTopology { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **SendObservers**

```csharp
public abstract ISendObserver SendObservers { get; }
```

#### Property Value

[ISendObserver](../../masstransit-abstractions/masstransit/isendobserver)<br/>

### **LogContext**

```csharp
public abstract ILogContext LogContext { get; set; }
```

#### Property Value

[ILogContext](../masstransit-logging/ilogcontext)<br/>

### **ReceiveLogContext**

```csharp
public abstract ILogContext ReceiveLogContext { get; }
```

#### Property Value

[ILogContext](../masstransit-logging/ilogcontext)<br/>

### **SendLogContext**

```csharp
public abstract ILogContext SendLogContext { get; }
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
public abstract IRetryPolicy SendTransportRetryPolicy { get; }
```

#### Property Value

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **ConsumerStopTimeout**

```csharp
public abstract Nullable<TimeSpan> ConsumerStopTimeout { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StopTimeout**

```csharp
public abstract Nullable<TimeSpan> StopTimeout { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Methods

### **CreateReceiveEndpointConfiguration(String, Action\<IReceiveEndpointConfigurator\>)**

Create a receive endpoint configuration

```csharp
IReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, Action<IReceiveEndpointConfigurator> configure)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configure` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IReceiveEndpointConfiguration](../masstransit-configuration/ireceiveendpointconfiguration)<br/>

### **ConnectReceiveEndpointContext(ReceiveEndpointContext)**

Called by the base ReceiveEndpointContext constructor so that the observer collections are connected to the bus observer

```csharp
ConnectHandle ConnectReceiveEndpointContext(ReceiveEndpointContext context)
```

#### Parameters

`context` [ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **Build()**

```csharp
IHost Build()
```

#### Returns

[IHost](../masstransit-transports/ihost)<br/>
