---

title: FutureRegistrationConfigurator<TFuture>

---

# FutureRegistrationConfigurator\<TFuture\>

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public class FutureRegistrationConfigurator<TFuture> : IFutureRegistrationConfigurator<TFuture>, IFutureRegistrationConfigurator
```

#### Type Parameters

`TFuture`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureRegistrationConfigurator\<TFuture\>](../masstransit-dependencyinjection-registration/futureregistrationconfigurator-1)<br/>
Implements [IFutureRegistrationConfigurator\<TFuture\>](../masstransit/ifutureregistrationconfigurator-1), [IFutureRegistrationConfigurator](../masstransit/ifutureregistrationconfigurator)

## Constructors

### **FutureRegistrationConfigurator(IRegistrationConfigurator, IFutureRegistration)**

```csharp
public FutureRegistrationConfigurator(IRegistrationConfigurator configurator, IFutureRegistration registration)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`registration` [IFutureRegistration](../masstransit-configuration/ifutureregistration)<br/>

## Methods

### **ExcludeFromConfigureEndpoints()**

```csharp
public void ExcludeFromConfigureEndpoints()
```

### **Endpoint(Action\<IEndpointRegistrationConfigurator\>)**

```csharp
public IFutureRegistrationConfigurator<TFuture> Endpoint(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IFutureRegistrationConfigurator\<TFuture\>](../masstransit/ifutureregistrationconfigurator-1)<br/>

### **Repository(Action\<ISagaRepositoryRegistrationConfigurator\<FutureState\>\>)**

```csharp
public IFutureRegistrationConfigurator<TFuture> Repository(Action<ISagaRepositoryRegistrationConfigurator<FutureState>> configure)
```

#### Parameters

`configure` [Action\<ISagaRepositoryRegistrationConfigurator\<FutureState\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IFutureRegistrationConfigurator\<TFuture\>](../masstransit/ifutureregistrationconfigurator-1)<br/>
