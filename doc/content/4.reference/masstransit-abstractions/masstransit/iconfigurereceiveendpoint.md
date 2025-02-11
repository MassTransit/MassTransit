---

title: IConfigureReceiveEndpoint

---

# IConfigureReceiveEndpoint

Namespace: MassTransit

Implement this interface, and register the implementation in the container as the interface
 type to apply configuration to all configured receive endpoints

```csharp
public interface IConfigureReceiveEndpoint
```

## Methods

### **Configure(String, IReceiveEndpointConfigurator)**

Configure the receive endpoint (called prior to any consumer, saga, or activity configuration)

```csharp
void Configure(string name, IReceiveEndpointConfigurator configurator)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>
