---

title: SqlReceiveEndpointConfigurationExtensions

---

# SqlReceiveEndpointConfigurationExtensions

Namespace: MassTransit

```csharp
public static class SqlReceiveEndpointConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlReceiveEndpointConfigurationExtensions](../masstransit/sqlreceiveendpointconfigurationextensions)

## Methods

### **ReceiveEndpoint(ISqlBusFactoryConfigurator, Action\<ISqlReceiveEndpointConfigurator\>)**

Declare a ReceiveEndpoint using a unique generated queue name. This queue defaults to auto-delete
 and non-durable. By default all services bus instances include a default receiveEndpoint that is
 of this type (created automatically upon the first receiver binding).

```csharp
public static void ReceiveEndpoint(ISqlBusFactoryConfigurator configurator, Action<ISqlReceiveEndpointConfigurator> configure)
```

#### Parameters

`configurator` [ISqlBusFactoryConfigurator](../masstransit/isqlbusfactoryconfigurator)<br/>

`configure` [Action\<ISqlReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ReceiveEndpoint(ISqlBusFactoryConfigurator, IEndpointDefinition, Action\<ISqlReceiveEndpointConfigurator\>)**

Declare a receive endpoint using the endpoint .

```csharp
public static void ReceiveEndpoint(ISqlBusFactoryConfigurator configurator, IEndpointDefinition definition, Action<ISqlReceiveEndpointConfigurator> configure)
```

#### Parameters

`configurator` [ISqlBusFactoryConfigurator](../masstransit/isqlbusfactoryconfigurator)<br/>

`definition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

`configure` [Action\<ISqlReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
