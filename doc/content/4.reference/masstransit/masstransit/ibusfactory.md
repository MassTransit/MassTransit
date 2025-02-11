---

title: IBusFactory

---

# IBusFactory

Namespace: MassTransit

```csharp
public interface IBusFactory : ISpecification
```

Implements [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Methods

### **CreateBusEndpointConfiguration(Action\<IReceiveEndpointConfigurator\>)**

Create the bus endpoint configuration, which is used to create the bus

```csharp
IReceiveEndpointConfiguration CreateBusEndpointConfiguration(Action<IReceiveEndpointConfigurator> configure)
```

#### Parameters

`configure` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IReceiveEndpointConfiguration](../masstransit-configuration/ireceiveendpointconfiguration)<br/>
