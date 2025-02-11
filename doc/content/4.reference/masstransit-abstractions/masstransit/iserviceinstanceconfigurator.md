---

title: IServiceInstanceConfigurator

---

# IServiceInstanceConfigurator

Namespace: MassTransit

```csharp
public interface IServiceInstanceConfigurator : IReceiveConfigurator, IEndpointConfigurationObserverConnector, IOptionsSet
```

Implements [IReceiveConfigurator](../masstransit/ireceiveconfigurator), [IEndpointConfigurationObserverConnector](../masstransit/iendpointconfigurationobserverconnector), [IOptionsSet](../masstransit-configuration/ioptionsset)

## Properties

### **EndpointNameFormatter**

```csharp
public abstract IEndpointNameFormatter EndpointNameFormatter { get; }
```

#### Property Value

[IEndpointNameFormatter](../masstransit/iendpointnameformatter)<br/>

### **InstanceAddress**

If the InstanceEndpoint is enabled, the address of the instance endpoint

```csharp
public abstract Uri InstanceAddress { get; }
```

#### Property Value

Uri<br/>

### **BusConfigurator**

```csharp
public abstract IReceiveConfigurator BusConfigurator { get; }
```

#### Property Value

[IReceiveConfigurator](../masstransit/ireceiveconfigurator)<br/>

### **InstanceEndpointConfigurator**

```csharp
public abstract IReceiveEndpointConfigurator InstanceEndpointConfigurator { get; }
```

#### Property Value

[IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>

## Methods

### **AddSpecification(ISpecification)**

Add a specification for validation

```csharp
void AddSpecification(ISpecification specification)
```

#### Parameters

`specification` [ISpecification](../masstransit/ispecification)<br/>
