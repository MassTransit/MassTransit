---

title: IEndpointConfigurationObserverConnector

---

# IEndpointConfigurationObserverConnector

Namespace: MassTransit

```csharp
public interface IEndpointConfigurationObserverConnector
```

## Methods

### **ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver)**

Connect a configuration observer to the bus configurator, which is invoked as consumers are configured.

```csharp
ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
```

#### Parameters

`observer` [IEndpointConfigurationObserver](../masstransit/iendpointconfigurationobserver)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>
