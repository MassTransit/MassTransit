---

title: ActivityRegistrationConfigurator<TActivity, TArguments, TLog>

---

# ActivityRegistrationConfigurator\<TActivity, TArguments, TLog\>

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public class ActivityRegistrationConfigurator<TActivity, TArguments, TLog> : IActivityRegistrationConfigurator<TActivity, TArguments, TLog>, IActivityRegistrationConfigurator
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ActivityRegistrationConfigurator\<TActivity, TArguments, TLog\>](../masstransit-dependencyinjection-registration/activityregistrationconfigurator-3)<br/>
Implements [IActivityRegistrationConfigurator\<TActivity, TArguments, TLog\>](../../masstransit-abstractions/masstransit/iactivityregistrationconfigurator-3), [IActivityRegistrationConfigurator](../../masstransit-abstractions/masstransit/iactivityregistrationconfigurator)

## Constructors

### **ActivityRegistrationConfigurator(IRegistrationConfigurator, IActivityRegistration)**

```csharp
public ActivityRegistrationConfigurator(IRegistrationConfigurator configurator, IActivityRegistration registration)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`registration` [IActivityRegistration](../masstransit-configuration/iactivityregistration)<br/>

## Methods

### **Endpoints(Action\<IEndpointRegistrationConfigurator\>, Action\<IEndpointRegistrationConfigurator\>)**

```csharp
public void Endpoints(Action<IEndpointRegistrationConfigurator> configureExecute, Action<IEndpointRegistrationConfigurator> configureCompensate)
```

#### Parameters

`configureExecute` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`configureCompensate` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ExecuteEndpoint(Action\<IEndpointRegistrationConfigurator\>)**

```csharp
public IActivityRegistrationConfigurator ExecuteEndpoint(Action<IEndpointRegistrationConfigurator> configureExecute)
```

#### Parameters

`configureExecute` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IActivityRegistrationConfigurator](../../masstransit-abstractions/masstransit/iactivityregistrationconfigurator)<br/>

### **CompensateEndpoint(Action\<IEndpointRegistrationConfigurator\>)**

```csharp
public IActivityRegistrationConfigurator CompensateEndpoint(Action<IEndpointRegistrationConfigurator> configureCompensate)
```

#### Parameters

`configureCompensate` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IActivityRegistrationConfigurator](../../masstransit-abstractions/masstransit/iactivityregistrationconfigurator)<br/>

### **ExcludeFromConfigureEndpoints()**

```csharp
public void ExcludeFromConfigureEndpoints()
```
