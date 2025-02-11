---

title: IServiceInstanceConfigurator<TEndpointConfigurator>

---

# IServiceInstanceConfigurator\<TEndpointConfigurator\>

Namespace: MassTransit

```csharp
public interface IServiceInstanceConfigurator<TEndpointConfigurator> : IServiceInstanceConfigurator, IReceiveConfigurator, IEndpointConfigurationObserverConnector, IOptionsSet, IReceiveConfigurator<TEndpointConfigurator>
```

#### Type Parameters

`TEndpointConfigurator`<br/>

Implements [IServiceInstanceConfigurator](../masstransit/iserviceinstanceconfigurator), [IReceiveConfigurator](../masstransit/ireceiveconfigurator), [IEndpointConfigurationObserverConnector](../masstransit/iendpointconfigurationobserverconnector), [IOptionsSet](../masstransit-configuration/ioptionsset), [IReceiveConfigurator\<TEndpointConfigurator\>](../masstransit/ireceiveconfigurator-1)

## Properties

### **BusConfigurator**

```csharp
public abstract IReceiveConfigurator<TEndpointConfigurator> BusConfigurator { get; }
```

#### Property Value

[IReceiveConfigurator\<TEndpointConfigurator\>](../masstransit/ireceiveconfigurator-1)<br/>

### **InstanceEndpointConfigurator**

```csharp
public abstract TEndpointConfigurator InstanceEndpointConfigurator { get; }
```

#### Property Value

TEndpointConfigurator<br/>
