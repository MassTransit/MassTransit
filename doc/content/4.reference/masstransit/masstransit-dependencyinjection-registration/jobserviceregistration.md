---

title: JobServiceRegistration

---

# JobServiceRegistration

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public class JobServiceRegistration : IJobServiceRegistration, IRegistration
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobServiceRegistration](../masstransit-dependencyinjection-registration/jobserviceregistration)<br/>
Implements [IJobServiceRegistration](../masstransit-configuration/ijobserviceregistration), [IRegistration](../masstransit-configuration/iregistration)

## Properties

### **Type**

```csharp
public Type Type { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **IncludeInConfigureEndpoints**

```csharp
public bool IncludeInConfigureEndpoints { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **EndpointRegistrationConfigurator**

```csharp
public IEndpointRegistrationConfigurator EndpointRegistrationConfigurator { get; }
```

#### Property Value

[IEndpointRegistrationConfigurator](../../masstransit-abstractions/masstransit/iendpointregistrationconfigurator)<br/>

### **EndpointDefinition**

```csharp
public IEndpointDefinition EndpointDefinition { get; }
```

#### Property Value

[IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

## Constructors

### **JobServiceRegistration()**

```csharp
public JobServiceRegistration()
```

## Methods

### **AddConfigureAction(Action\<JobConsumerOptions\>)**

```csharp
public void AddConfigureAction(Action<JobConsumerOptions> configure)
```

#### Parameters

`configure` [Action\<JobConsumerOptions\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **AddReceiveEndpointDependency(IReceiveEndpointConfigurator)**

```csharp
public void AddReceiveEndpointDependency(IReceiveEndpointConfigurator dependency)
```

#### Parameters

`dependency` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

### **Configure(IServiceInstanceConfigurator, IRegistrationContext)**

```csharp
public void Configure(IServiceInstanceConfigurator instanceConfigurator, IRegistrationContext context)
```

#### Parameters

`instanceConfigurator` [IServiceInstanceConfigurator](../../masstransit-abstractions/masstransit/iserviceinstanceconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
