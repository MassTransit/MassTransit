---

title: IJobServiceRegistration

---

# IJobServiceRegistration

Namespace: MassTransit.Configuration

```csharp
public interface IJobServiceRegistration : IRegistration
```

Implements [IRegistration](../masstransit-configuration/iregistration)

## Properties

### **EndpointRegistrationConfigurator**

```csharp
public abstract IEndpointRegistrationConfigurator EndpointRegistrationConfigurator { get; }
```

#### Property Value

[IEndpointRegistrationConfigurator](../../masstransit-abstractions/masstransit/iendpointregistrationconfigurator)<br/>

### **EndpointDefinition**

```csharp
public abstract IEndpointDefinition EndpointDefinition { get; }
```

#### Property Value

[IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

## Methods

### **AddConfigureAction(Action\<JobConsumerOptions\>)**

```csharp
void AddConfigureAction(Action<JobConsumerOptions> configure)
```

#### Parameters

`configure` [Action\<JobConsumerOptions\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **AddReceiveEndpointDependency(IReceiveEndpointConfigurator)**

```csharp
void AddReceiveEndpointDependency(IReceiveEndpointConfigurator dependency)
```

#### Parameters

`dependency` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

### **Configure(IServiceInstanceConfigurator, IRegistrationContext)**

```csharp
void Configure(IServiceInstanceConfigurator instanceConfigurator, IRegistrationContext context)
```

#### Parameters

`instanceConfigurator` [IServiceInstanceConfigurator](../../masstransit-abstractions/masstransit/iserviceinstanceconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
