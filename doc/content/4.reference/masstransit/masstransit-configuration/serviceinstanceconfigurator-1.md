---

title: ServiceInstanceConfigurator<TEndpointConfigurator>

---

# ServiceInstanceConfigurator\<TEndpointConfigurator\>

Namespace: MassTransit.Configuration

```csharp
public class ServiceInstanceConfigurator<TEndpointConfigurator> : IServiceInstanceConfigurator<TEndpointConfigurator>, IServiceInstanceConfigurator, IReceiveConfigurator, IEndpointConfigurationObserverConnector, IOptionsSet, IReceiveConfigurator<TEndpointConfigurator>
```

#### Type Parameters

`TEndpointConfigurator`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ServiceInstanceConfigurator\<TEndpointConfigurator\>](../masstransit-configuration/serviceinstanceconfigurator-1)<br/>
Implements [IServiceInstanceConfigurator\<TEndpointConfigurator\>](../../masstransit-abstractions/masstransit/iserviceinstanceconfigurator-1), [IServiceInstanceConfigurator](../../masstransit-abstractions/masstransit/iserviceinstanceconfigurator), [IReceiveConfigurator](../../masstransit-abstractions/masstransit/ireceiveconfigurator), [IEndpointConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iendpointconfigurationobserverconnector), [IOptionsSet](../../masstransit-abstractions/masstransit-configuration/ioptionsset), [IReceiveConfigurator\<TEndpointConfigurator\>](../../masstransit-abstractions/masstransit/ireceiveconfigurator-1)

## Properties

### **InstanceAddress**

```csharp
public Uri InstanceAddress { get; }
```

#### Property Value

Uri<br/>

### **BusConfigurator**

```csharp
public IReceiveConfigurator<TEndpointConfigurator> BusConfigurator { get; }
```

#### Property Value

[IReceiveConfigurator\<TEndpointConfigurator\>](../../masstransit-abstractions/masstransit/ireceiveconfigurator-1)<br/>

### **InstanceEndpointConfigurator**

```csharp
public TEndpointConfigurator InstanceEndpointConfigurator { get; }
```

#### Property Value

TEndpointConfigurator<br/>

### **EndpointNameFormatter**

```csharp
public IEndpointNameFormatter EndpointNameFormatter { get; }
```

#### Property Value

[IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

## Constructors

### **ServiceInstanceConfigurator(IReceiveConfigurator\<TEndpointConfigurator\>, ServiceInstanceOptions, TEndpointConfigurator)**

```csharp
public ServiceInstanceConfigurator(IReceiveConfigurator<TEndpointConfigurator> configurator, ServiceInstanceOptions options, TEndpointConfigurator instanceEndpointConfigurator)
```

#### Parameters

`configurator` [IReceiveConfigurator\<TEndpointConfigurator\>](../../masstransit-abstractions/masstransit/ireceiveconfigurator-1)<br/>

`options` [ServiceInstanceOptions](../masstransit/serviceinstanceoptions)<br/>

`instanceEndpointConfigurator` TEndpointConfigurator<br/>

## Methods

### **AddSpecification(ISpecification)**

```csharp
public void AddSpecification(ISpecification specification)
```

#### Parameters

`specification` [ISpecification](../../masstransit-abstractions/masstransit/ispecification)<br/>

### **Options\<T\>(Action\<T\>)**

```csharp
public T Options<T>(Action<T> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configure` [Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

T<br/>

### **Options\<T\>(T, Action\<T\>)**

```csharp
public T Options<T>(T options, Action<T> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`options` T<br/>

`configure` [Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

T<br/>

### **TryGetOptions\<T\>(T)**

```csharp
public bool TryGetOptions<T>(out T options)
```

#### Type Parameters

`T`<br/>

#### Parameters

`options` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **SelectOptions\<T\>()**

```csharp
public IEnumerable<T> SelectOptions<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **ReceiveEndpoint(IEndpointDefinition, IEndpointNameFormatter, Action\<TEndpointConfigurator\>)**

```csharp
public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<TEndpointConfigurator> configureEndpoint)
```

#### Parameters

`definition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

`configureEndpoint` [Action\<TEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ReceiveEndpoint(String, Action\<TEndpointConfigurator\>)**

```csharp
public void ReceiveEndpoint(string queueName, Action<TEndpointConfigurator> configureEndpoint)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configureEndpoint` [Action\<TEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver)**

```csharp
public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
```

#### Parameters

`observer` [IEndpointConfigurationObserver](../../masstransit-abstractions/masstransit/iendpointconfigurationobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
