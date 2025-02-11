---

title: IFutureRegistrationConfigurator

---

# IFutureRegistrationConfigurator

Namespace: MassTransit

```csharp
public interface IFutureRegistrationConfigurator
```

## Methods

### **Endpoint(Action\<IEndpointRegistrationConfigurator\>)**

```csharp
IFutureRegistrationConfigurator Endpoint(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IFutureRegistrationConfigurator](../masstransit/ifutureregistrationconfigurator)<br/>

### **ExcludeFromConfigureEndpoints()**

```csharp
void ExcludeFromConfigureEndpoints()
```
