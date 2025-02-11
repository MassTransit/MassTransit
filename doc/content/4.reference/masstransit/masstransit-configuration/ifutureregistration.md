---

title: IFutureRegistration

---

# IFutureRegistration

Namespace: MassTransit.Configuration

```csharp
public interface IFutureRegistration : IRegistration
```

Implements [IRegistration](../masstransit-configuration/iregistration)

## Methods

### **Configure(IReceiveEndpointConfigurator, IRegistrationContext)**

```csharp
void Configure(IReceiveEndpointConfigurator configurator, IRegistrationContext context)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

### **GetDefinition(IRegistrationContext)**

```csharp
IFutureDefinition GetDefinition(IRegistrationContext context)
```

#### Parameters

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

#### Returns

[IFutureDefinition](../../masstransit-abstractions/masstransit/ifuturedefinition)<br/>
