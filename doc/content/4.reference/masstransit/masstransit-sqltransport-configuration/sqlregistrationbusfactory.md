---

title: SqlRegistrationBusFactory

---

# SqlRegistrationBusFactory

Namespace: MassTransit.SqlTransport.Configuration

```csharp
public class SqlRegistrationBusFactory : TransportRegistrationBusFactory<ISqlReceiveEndpointConfigurator>, IRegistrationBusFactory
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransportRegistrationBusFactory\<ISqlReceiveEndpointConfigurator\>](../masstransit-configuration/transportregistrationbusfactory-1) → [SqlRegistrationBusFactory](../masstransit-sqltransport-configuration/sqlregistrationbusfactory)<br/>
Implements [IRegistrationBusFactory](../masstransit/iregistrationbusfactory)

## Constructors

### **SqlRegistrationBusFactory(Action\<IBusRegistrationContext, ISqlBusFactoryConfigurator\>)**

```csharp
public SqlRegistrationBusFactory(Action<IBusRegistrationContext, ISqlBusFactoryConfigurator> configure)
```

#### Parameters

`configure` [Action\<IBusRegistrationContext, ISqlBusFactoryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

## Methods

### **CreateBus(IBusRegistrationContext, IEnumerable\<IBusInstanceSpecification\>, String)**

```csharp
public IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications, string busName)
```

#### Parameters

`context` [IBusRegistrationContext](../masstransit/ibusregistrationcontext)<br/>

`specifications` [IEnumerable\<IBusInstanceSpecification\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`busName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IBusInstance](../masstransit-transports/ibusinstance)<br/>
