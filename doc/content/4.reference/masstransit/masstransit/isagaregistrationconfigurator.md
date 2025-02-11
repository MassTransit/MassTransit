---

title: ISagaRegistrationConfigurator

---

# ISagaRegistrationConfigurator

Namespace: MassTransit

```csharp
public interface ISagaRegistrationConfigurator
```

## Methods

### **Endpoint(Action\<IEndpointRegistrationConfigurator\>)**

```csharp
ISagaRegistrationConfigurator Endpoint(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ISagaRegistrationConfigurator](../masstransit/isagaregistrationconfigurator)<br/>

### **ExcludeFromConfigureEndpoints()**

```csharp
void ExcludeFromConfigureEndpoints()
```
