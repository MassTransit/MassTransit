---

title: IConsumerRegistration

---

# IConsumerRegistration

Namespace: MassTransit.Configuration

```csharp
public interface IConsumerRegistration : IRegistration
```

Implements [IRegistration](../masstransit-configuration/iregistration)

## Methods

### **AddConfigureAction\<T\>(Action\<IRegistrationContext, IConsumerConfigurator\<T\>\>)**

```csharp
void AddConfigureAction<T>(Action<IRegistrationContext, IConsumerConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configure` [Action\<IRegistrationContext, IConsumerConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

### **Configure(IReceiveEndpointConfigurator, IRegistrationContext)**

```csharp
void Configure(IReceiveEndpointConfigurator configurator, IRegistrationContext context)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

### **GetDefinition(IRegistrationContext)**

```csharp
IConsumerDefinition GetDefinition(IRegistrationContext context)
```

#### Parameters

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

#### Returns

[IConsumerDefinition](../../masstransit-abstractions/masstransit/iconsumerdefinition)<br/>

### **GetConsumerRegistrationConfigurator(IRegistrationConfigurator)**

```csharp
IConsumerRegistrationConfigurator GetConsumerRegistrationConfigurator(IRegistrationConfigurator registrationConfigurator)
```

#### Parameters

`registrationConfigurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

#### Returns

[IConsumerRegistrationConfigurator](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator)<br/>
