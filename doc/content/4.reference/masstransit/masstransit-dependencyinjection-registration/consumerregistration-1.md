---

title: ConsumerRegistration<TConsumer>

---

# ConsumerRegistration\<TConsumer\>

Namespace: MassTransit.DependencyInjection.Registration

A consumer registration represents a single consumer, which will be resolved from the container using the scope
 provider. The consumer definition, if present, is loaded from the container and used to configure the consumer
 within the receive endpoint.

```csharp
public class ConsumerRegistration<TConsumer> : IConsumerRegistration, IRegistration
```

#### Type Parameters

`TConsumer`<br/>
The consumer type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerRegistration\<TConsumer\>](../masstransit-dependencyinjection-registration/consumerregistration-1)<br/>
Implements [IConsumerRegistration](../masstransit-configuration/iconsumerregistration), [IRegistration](../masstransit-configuration/iregistration)

## Properties

### **Type**

```csharp
public Type Type { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **IncludeInConfigureEndpoints**

```csharp
public bool IncludeInConfigureEndpoints { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **ConsumerRegistration(IContainerSelector)**

```csharp
public ConsumerRegistration(IContainerSelector selector)
```

#### Parameters

`selector` [IContainerSelector](../masstransit-configuration/icontainerselector)<br/>

## Methods

### **GetConsumerRegistrationConfigurator(IRegistrationConfigurator)**

```csharp
public IConsumerRegistrationConfigurator GetConsumerRegistrationConfigurator(IRegistrationConfigurator registrationConfigurator)
```

#### Parameters

`registrationConfigurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

#### Returns

[IConsumerRegistrationConfigurator](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator)<br/>
