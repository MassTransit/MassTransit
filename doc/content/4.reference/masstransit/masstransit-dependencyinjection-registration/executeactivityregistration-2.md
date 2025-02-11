---

title: ExecuteActivityRegistration<TActivity, TArguments>

---

# ExecuteActivityRegistration\<TActivity, TArguments\>

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public class ExecuteActivityRegistration<TActivity, TArguments> : IExecuteActivityRegistration, IRegistration
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExecuteActivityRegistration\<TActivity, TArguments\>](../masstransit-dependencyinjection-registration/executeactivityregistration-2)<br/>
Implements [IExecuteActivityRegistration](../masstransit-configuration/iexecuteactivityregistration), [IRegistration](../masstransit-configuration/iregistration)

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

### **ExecuteActivityRegistration(IContainerSelector)**

```csharp
public ExecuteActivityRegistration(IContainerSelector selector)
```

#### Parameters

`selector` [IContainerSelector](../masstransit-configuration/icontainerselector)<br/>

## Methods

### **Configure(IReceiveEndpointConfigurator, IRegistrationContext)**

```csharp
public void Configure(IReceiveEndpointConfigurator configurator, IRegistrationContext context)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
