---

title: IInMemoryHostConfiguration

---

# IInMemoryHostConfiguration

Namespace: MassTransit.InMemoryTransport.Configuration

```csharp
public interface IInMemoryHostConfiguration : IHostConfiguration, IEndpointConfigurationObserverConnector, IReceiveObserverConnector, IConsumeObserverConnector, IPublishObserverConnector, ISendObserverConnector, ISpecification, IReceiveConfigurator<IInMemoryReceiveEndpointConfigurator>, IReceiveConfigurator
```

Implements [IHostConfiguration](../masstransit-configuration/ihostconfiguration), [IEndpointConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iendpointconfigurationobserverconnector), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IReceiveConfigurator\<IInMemoryReceiveEndpointConfigurator\>](../../masstransit-abstractions/masstransit/ireceiveconfigurator-1), [IReceiveConfigurator](../../masstransit-abstractions/masstransit/ireceiveconfigurator)

## Properties

### **BaseAddress**

Set the host's base address

```csharp
public abstract Uri BaseAddress { set; }
```

#### Property Value

Uri<br/>

### **Configurator**

```csharp
public abstract IInMemoryHostConfigurator Configurator { get; }
```

#### Property Value

[IInMemoryHostConfigurator](../masstransit/iinmemoryhostconfigurator)<br/>

### **TransportProvider**

```csharp
public abstract IInMemoryTransportProvider TransportProvider { get; }
```

#### Property Value

[IInMemoryTransportProvider](../masstransit-inmemorytransport/iinmemorytransportprovider)<br/>

### **Topology**

```csharp
public abstract IInMemoryBusTopology Topology { get; }
```

#### Property Value

[IInMemoryBusTopology](../masstransit/iinmemorybustopology)<br/>

## Methods

### **ApplyEndpointDefinition(IInMemoryReceiveEndpointConfigurator, IEndpointDefinition)**

```csharp
void ApplyEndpointDefinition(IInMemoryReceiveEndpointConfigurator configurator, IEndpointDefinition definition)
```

#### Parameters

`configurator` [IInMemoryReceiveEndpointConfigurator](../masstransit/iinmemoryreceiveendpointconfigurator)<br/>

`definition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

### **CreateReceiveEndpointConfiguration(String, Action\<IInMemoryReceiveEndpointConfigurator\>)**

```csharp
IInMemoryReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, Action<IInMemoryReceiveEndpointConfigurator> configure)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configure` [Action\<IInMemoryReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IInMemoryReceiveEndpointConfiguration](../masstransit-inmemorytransport-configuration/iinmemoryreceiveendpointconfiguration)<br/>

### **CreateReceiveEndpointConfiguration(String, IInMemoryEndpointConfiguration, Action\<IInMemoryReceiveEndpointConfigurator\>)**

```csharp
IInMemoryReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, IInMemoryEndpointConfiguration endpointConfiguration, Action<IInMemoryReceiveEndpointConfigurator> configure)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`endpointConfiguration` [IInMemoryEndpointConfiguration](../masstransit-inmemorytransport-configuration/iinmemoryendpointconfiguration)<br/>

`configure` [Action\<IInMemoryReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IInMemoryReceiveEndpointConfiguration](../masstransit-inmemorytransport-configuration/iinmemoryreceiveendpointconfiguration)<br/>
