---

title: ISagaRegistration

---

# ISagaRegistration

Namespace: MassTransit.Configuration

```csharp
public interface ISagaRegistration : IRegistration
```

Implements [IRegistration](../masstransit-configuration/iregistration)

## Methods

### **AddConfigureAction\<T\>(Action\<IRegistrationContext, ISagaConfigurator\<T\>\>)**

```csharp
void AddConfigureAction<T>(Action<IRegistrationContext, ISagaConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configure` [Action\<IRegistrationContext, ISagaConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

### **Configure(IReceiveEndpointConfigurator, IRegistrationContext)**

```csharp
void Configure(IReceiveEndpointConfigurator configurator, IRegistrationContext context)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

### **GetDefinition(IRegistrationContext)**

```csharp
ISagaDefinition GetDefinition(IRegistrationContext context)
```

#### Parameters

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

#### Returns

[ISagaDefinition](../../masstransit-abstractions/masstransit/isagadefinition)<br/>
