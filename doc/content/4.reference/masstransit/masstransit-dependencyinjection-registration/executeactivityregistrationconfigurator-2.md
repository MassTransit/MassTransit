---

title: ExecuteActivityRegistrationConfigurator<TActivity, TArguments>

---

# ExecuteActivityRegistrationConfigurator\<TActivity, TArguments\>

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public class ExecuteActivityRegistrationConfigurator<TActivity, TArguments> : IExecuteActivityRegistrationConfigurator<TActivity, TArguments>, IExecuteActivityRegistrationConfigurator
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExecuteActivityRegistrationConfigurator\<TActivity, TArguments\>](../masstransit-dependencyinjection-registration/executeactivityregistrationconfigurator-2)<br/>
Implements [IExecuteActivityRegistrationConfigurator\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityregistrationconfigurator-2), [IExecuteActivityRegistrationConfigurator](../../masstransit-abstractions/masstransit/iexecuteactivityregistrationconfigurator)

## Constructors

### **ExecuteActivityRegistrationConfigurator(IRegistrationConfigurator, IExecuteActivityRegistration)**

```csharp
public ExecuteActivityRegistrationConfigurator(IRegistrationConfigurator configurator, IExecuteActivityRegistration registration)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`registration` [IExecuteActivityRegistration](../masstransit-configuration/iexecuteactivityregistration)<br/>

## Methods

### **Endpoint(Action\<IEndpointRegistrationConfigurator\>)**

```csharp
public void Endpoint(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ExcludeFromConfigureEndpoints()**

```csharp
public void ExcludeFromConfigureEndpoints()
```
