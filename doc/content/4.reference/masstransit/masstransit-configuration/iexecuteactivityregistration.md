---

title: IExecuteActivityRegistration

---

# IExecuteActivityRegistration

Namespace: MassTransit.Configuration

An execute activity, which doesn't have compensation

```csharp
public interface IExecuteActivityRegistration : IRegistration
```

Implements [IRegistration](../masstransit-configuration/iregistration)

## Methods

### **AddConfigureAction\<T, TArguments\>(Action\<IRegistrationContext, IExecuteActivityConfigurator\<T, TArguments\>\>)**

```csharp
void AddConfigureAction<T, TArguments>(Action<IRegistrationContext, IExecuteActivityConfigurator<T, TArguments>> configure)
```

#### Type Parameters

`T`<br/>

`TArguments`<br/>

#### Parameters

`configure` [Action\<IRegistrationContext, IExecuteActivityConfigurator\<T, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

### **Configure(IReceiveEndpointConfigurator, IRegistrationContext)**

```csharp
void Configure(IReceiveEndpointConfigurator configurator, IRegistrationContext context)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

### **GetDefinition(IRegistrationContext)**

```csharp
IExecuteActivityDefinition GetDefinition(IRegistrationContext context)
```

#### Parameters

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

#### Returns

[IExecuteActivityDefinition](../../masstransit-abstractions/masstransit/iexecuteactivitydefinition)<br/>
