---

title: InMemoryRegistrationBusFactory

---

# InMemoryRegistrationBusFactory

Namespace: MassTransit.InMemoryTransport.Configuration

```csharp
public class InMemoryRegistrationBusFactory : TransportRegistrationBusFactory<IInMemoryReceiveEndpointConfigurator>, IRegistrationBusFactory
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransportRegistrationBusFactory\<IInMemoryReceiveEndpointConfigurator\>](../masstransit-configuration/transportregistrationbusfactory-1) → [InMemoryRegistrationBusFactory](../masstransit-inmemorytransport-configuration/inmemoryregistrationbusfactory)<br/>
Implements [IRegistrationBusFactory](../masstransit/iregistrationbusfactory)

## Constructors

### **InMemoryRegistrationBusFactory(Uri, Action\<IBusRegistrationContext, IInMemoryBusFactoryConfigurator\>)**

```csharp
public InMemoryRegistrationBusFactory(Uri baseAddress, Action<IBusRegistrationContext, IInMemoryBusFactoryConfigurator> configure)
```

#### Parameters

`baseAddress` Uri<br/>

`configure` [Action\<IBusRegistrationContext, IInMemoryBusFactoryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

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
