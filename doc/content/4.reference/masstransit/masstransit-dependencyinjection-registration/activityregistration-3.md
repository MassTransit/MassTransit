---

title: ActivityRegistration<TActivity, TArguments, TLog>

---

# ActivityRegistration\<TActivity, TArguments, TLog\>

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public class ActivityRegistration<TActivity, TArguments, TLog> : IActivityRegistration, IRegistration
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ActivityRegistration\<TActivity, TArguments, TLog\>](../masstransit-dependencyinjection-registration/activityregistration-3)<br/>
Implements [IActivityRegistration](../masstransit-configuration/iactivityregistration), [IRegistration](../masstransit-configuration/iregistration)

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

### **ActivityRegistration(IContainerSelector)**

```csharp
public ActivityRegistration(IContainerSelector selector)
```

#### Parameters

`selector` [IContainerSelector](../masstransit-configuration/icontainerselector)<br/>

## Methods

### **AddConfigureAction\<T, TA\>(Action\<IRegistrationContext, IExecuteActivityConfigurator\<T, TA\>\>)**

```csharp
public void AddConfigureAction<T, TA>(Action<IRegistrationContext, IExecuteActivityConfigurator<T, TA>> configure)
```

#### Type Parameters

`T`<br/>

`TA`<br/>

#### Parameters

`configure` [Action\<IRegistrationContext, IExecuteActivityConfigurator\<T, TA\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

### **AddConfigureAction\<T, TL\>(Action\<IRegistrationContext, ICompensateActivityConfigurator\<T, TL\>\>)**

```csharp
public void AddConfigureAction<T, TL>(Action<IRegistrationContext, ICompensateActivityConfigurator<T, TL>> configure)
```

#### Type Parameters

`T`<br/>

`TL`<br/>

#### Parameters

`configure` [Action\<IRegistrationContext, ICompensateActivityConfigurator\<T, TL\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

### **Configure(IReceiveEndpointConfigurator, IReceiveEndpointConfigurator, IRegistrationContext)**

```csharp
public void Configure(IReceiveEndpointConfigurator executeEndpointConfigurator, IReceiveEndpointConfigurator compensateEndpointConfigurator, IRegistrationContext context)
```

#### Parameters

`executeEndpointConfigurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`compensateEndpointConfigurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

### **ConfigureCompensate(IReceiveEndpointConfigurator, IRegistrationContext)**

```csharp
public void ConfigureCompensate(IReceiveEndpointConfigurator configurator, IRegistrationContext context)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

### **ConfigureExecute(IReceiveEndpointConfigurator, IRegistrationContext, Uri)**

```csharp
public void ConfigureExecute(IReceiveEndpointConfigurator configurator, IRegistrationContext context, Uri compensateAddress)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`compensateAddress` Uri<br/>
