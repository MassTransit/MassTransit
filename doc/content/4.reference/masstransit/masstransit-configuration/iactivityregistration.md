---

title: IActivityRegistration

---

# IActivityRegistration

Namespace: MassTransit.Configuration

An activity, which must be configured on two separate receive endpoints

```csharp
public interface IActivityRegistration : IRegistration
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

### **AddConfigureAction\<T, TLog\>(Action\<IRegistrationContext, ICompensateActivityConfigurator\<T, TLog\>\>)**

```csharp
void AddConfigureAction<T, TLog>(Action<IRegistrationContext, ICompensateActivityConfigurator<T, TLog>> configure)
```

#### Type Parameters

`T`<br/>

`TLog`<br/>

#### Parameters

`configure` [Action\<IRegistrationContext, ICompensateActivityConfigurator\<T, TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

### **Configure(IReceiveEndpointConfigurator, IReceiveEndpointConfigurator, IRegistrationContext)**

```csharp
void Configure(IReceiveEndpointConfigurator executeEndpointConfigurator, IReceiveEndpointConfigurator compensateEndpointConfigurator, IRegistrationContext context)
```

#### Parameters

`executeEndpointConfigurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`compensateEndpointConfigurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

### **GetDefinition(IRegistrationContext)**

```csharp
IActivityDefinition GetDefinition(IRegistrationContext context)
```

#### Parameters

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

#### Returns

[IActivityDefinition](../../masstransit-abstractions/masstransit/iactivitydefinition)<br/>

### **ConfigureCompensate(IReceiveEndpointConfigurator, IRegistrationContext)**

```csharp
void ConfigureCompensate(IReceiveEndpointConfigurator configurator, IRegistrationContext context)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

### **ConfigureExecute(IReceiveEndpointConfigurator, IRegistrationContext, Uri)**

```csharp
void ConfigureExecute(IReceiveEndpointConfigurator configurator, IRegistrationContext context, Uri compensateAddress)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`compensateAddress` Uri<br/>
