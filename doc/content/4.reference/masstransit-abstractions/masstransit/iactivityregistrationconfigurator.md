---

title: IActivityRegistrationConfigurator

---

# IActivityRegistrationConfigurator

Namespace: MassTransit

```csharp
public interface IActivityRegistrationConfigurator
```

## Methods

### **Endpoints(Action\<IEndpointRegistrationConfigurator\>, Action\<IEndpointRegistrationConfigurator\>)**

Configure both the execute and compensate endpoints in a single call. Separate calls have been added, which
 may ultimately cause this method to be deprecated.

```csharp
void Endpoints(Action<IEndpointRegistrationConfigurator> configureExecute, Action<IEndpointRegistrationConfigurator> configureCompensate)
```

#### Parameters

`configureExecute` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`configureCompensate` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ExecuteEndpoint(Action\<IEndpointRegistrationConfigurator\>)**

Configure the activity's execute endpoint

```csharp
IActivityRegistrationConfigurator ExecuteEndpoint(Action<IEndpointRegistrationConfigurator> configureExecute)
```

#### Parameters

`configureExecute` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IActivityRegistrationConfigurator](../masstransit/iactivityregistrationconfigurator)<br/>

### **CompensateEndpoint(Action\<IEndpointRegistrationConfigurator\>)**

Configure the activity's compensate endpoint

```csharp
IActivityRegistrationConfigurator CompensateEndpoint(Action<IEndpointRegistrationConfigurator> configureCompensate)
```

#### Parameters

`configureCompensate` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IActivityRegistrationConfigurator](../masstransit/iactivityregistrationconfigurator)<br/>

### **ExcludeFromConfigureEndpoints()**

```csharp
void ExcludeFromConfigureEndpoints()
```
