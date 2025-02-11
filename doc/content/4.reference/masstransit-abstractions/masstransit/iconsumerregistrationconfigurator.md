---

title: IConsumerRegistrationConfigurator

---

# IConsumerRegistrationConfigurator

Namespace: MassTransit

```csharp
public interface IConsumerRegistrationConfigurator
```

## Methods

### **Endpoint(Action\<IEndpointRegistrationConfigurator\>)**

```csharp
void Endpoint(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ExcludeFromConfigureEndpoints()**

```csharp
void ExcludeFromConfigureEndpoints()
```
