---

title: FutureRegistration<TFuture>

---

# FutureRegistration\<TFuture\>

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public class FutureRegistration<TFuture> : IFutureRegistration, IRegistration
```

#### Type Parameters

`TFuture`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureRegistration\<TFuture\>](../masstransit-dependencyinjection-registration/futureregistration-1)<br/>
Implements [IFutureRegistration](../masstransit-configuration/ifutureregistration), [IRegistration](../masstransit-configuration/iregistration)

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

### **FutureRegistration(IContainerSelector)**

```csharp
public FutureRegistration(IContainerSelector selector)
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

### **GetDefinition(IRegistrationContext)**

```csharp
public IFutureDefinition GetDefinition(IRegistrationContext context)
```

#### Parameters

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

#### Returns

[IFutureDefinition](../../masstransit-abstractions/masstransit/ifuturedefinition)<br/>
