---

title: ConsumerRegistrationConfigurator<TConsumer>

---

# ConsumerRegistrationConfigurator\<TConsumer\>

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public class ConsumerRegistrationConfigurator<TConsumer> : IConsumerRegistrationConfigurator<TConsumer>, IConsumerRegistrationConfigurator
```

#### Type Parameters

`TConsumer`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerRegistrationConfigurator\<TConsumer\>](../masstransit-dependencyinjection-registration/consumerregistrationconfigurator-1)<br/>
Implements [IConsumerRegistrationConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator-1), [IConsumerRegistrationConfigurator](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator)

## Constructors

### **ConsumerRegistrationConfigurator(IRegistrationConfigurator, IConsumerRegistration)**

```csharp
public ConsumerRegistrationConfigurator(IRegistrationConfigurator configurator, IConsumerRegistration registration)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`registration` [IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

## Methods

### **Endpoint(Action\<IEndpointRegistrationConfigurator\>)**

```csharp
public void Endpoint(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ExcludeFromConfigureEndpoints()**

```csharp
public void ExcludeFromConfigureEndpoints()
```
