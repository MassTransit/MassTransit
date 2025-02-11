---

title: TransportRegistrationBusFactory<TEndpointConfigurator>

---

# TransportRegistrationBusFactory\<TEndpointConfigurator\>

Namespace: MassTransit.Configuration

```csharp
public abstract class TransportRegistrationBusFactory<TEndpointConfigurator> : IRegistrationBusFactory
```

#### Type Parameters

`TEndpointConfigurator`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransportRegistrationBusFactory\<TEndpointConfigurator\>](../masstransit-configuration/transportregistrationbusfactory-1)<br/>
Implements [IRegistrationBusFactory](../masstransit/iregistrationbusfactory)

## Methods

### **CreateBus(IBusRegistrationContext, IEnumerable\<IBusInstanceSpecification\>, String)**

```csharp
public abstract IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications, string busName)
```

#### Parameters

`context` [IBusRegistrationContext](../masstransit/ibusregistrationcontext)<br/>

`specifications` [IEnumerable\<IBusInstanceSpecification\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`busName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IBusInstance](../masstransit-transports/ibusinstance)<br/>

### **CreateBus\<T, TConfigurator\>(T, IBusRegistrationContext, Action\<IBusRegistrationContext, TConfigurator\>, IEnumerable\<IBusInstanceSpecification\>)**

```csharp
protected IBusInstance CreateBus<T, TConfigurator>(T configurator, IBusRegistrationContext context, Action<IBusRegistrationContext, TConfigurator> configure, IEnumerable<IBusInstanceSpecification> specifications)
```

#### Type Parameters

`T`<br/>

`TConfigurator`<br/>

#### Parameters

`configurator` T<br/>

`context` [IBusRegistrationContext](../masstransit/ibusregistrationcontext)<br/>

`configure` [Action\<IBusRegistrationContext, TConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

`specifications` [IEnumerable\<IBusInstanceSpecification\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

#### Returns

[IBusInstance](../masstransit-transports/ibusinstance)<br/>

### **CreateBusInstance(IBusControl, IHost\<TEndpointConfigurator\>, IHostConfiguration, IBusRegistrationContext)**

```csharp
protected IBusInstance CreateBusInstance(IBusControl bus, IHost<TEndpointConfigurator> host, IHostConfiguration hostConfiguration, IBusRegistrationContext context)
```

#### Parameters

`bus` [IBusControl](../../masstransit-abstractions/masstransit/ibuscontrol)<br/>

`host` [IHost\<TEndpointConfigurator\>](../masstransit-transports/ihost-1)<br/>

`hostConfiguration` [IHostConfiguration](../masstransit-configuration/ihostconfiguration)<br/>

`context` [IBusRegistrationContext](../masstransit/ibusregistrationcontext)<br/>

#### Returns

[IBusInstance](../masstransit-transports/ibusinstance)<br/>
