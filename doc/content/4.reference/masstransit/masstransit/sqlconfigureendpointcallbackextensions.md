---

title: SqlConfigureEndpointCallbackExtensions

---

# SqlConfigureEndpointCallbackExtensions

Namespace: MassTransit

```csharp
public static class SqlConfigureEndpointCallbackExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlConfigureEndpointCallbackExtensions](../masstransit/sqlconfigureendpointcallbackextensions)

## Methods

### **AddSqlConfigureEndpointCallback(IEndpointRegistrationConfigurator, Action\<IRegistrationContext, ISqlReceiveEndpointConfigurator\>)**

Add an Azure Service Bus specific configure callback to the endpoint.

```csharp
public static void AddSqlConfigureEndpointCallback(IEndpointRegistrationConfigurator configurator, Action<IRegistrationContext, ISqlReceiveEndpointConfigurator> callback)
```

#### Parameters

`configurator` [IEndpointRegistrationConfigurator](../../masstransit-abstractions/masstransit/iendpointregistrationconfigurator)<br/>

`callback` [Action\<IRegistrationContext, ISqlReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

### **AddSqlConfigureEndpointsCallback(IBusRegistrationConfigurator, SqlConfigureEndpointsCallback)**

Add an Azure Service Bus specific configure callback for configured endpoints

```csharp
public static void AddSqlConfigureEndpointsCallback(IBusRegistrationConfigurator configurator, SqlConfigureEndpointsCallback callback)
```

#### Parameters

`configurator` [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>

`callback` [SqlConfigureEndpointsCallback](../masstransit/sqlconfigureendpointscallback)<br/>
