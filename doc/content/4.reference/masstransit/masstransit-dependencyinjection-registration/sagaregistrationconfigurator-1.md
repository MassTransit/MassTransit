---

title: SagaRegistrationConfigurator<TSaga>

---

# SagaRegistrationConfigurator\<TSaga\>

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public class SagaRegistrationConfigurator<TSaga> : ISagaRegistrationConfigurator<TSaga>, ISagaRegistrationConfigurator
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaRegistrationConfigurator\<TSaga\>](../masstransit-dependencyinjection-registration/sagaregistrationconfigurator-1)<br/>
Implements [ISagaRegistrationConfigurator\<TSaga\>](../masstransit/isagaregistrationconfigurator-1), [ISagaRegistrationConfigurator](../masstransit/isagaregistrationconfigurator)

## Constructors

### **SagaRegistrationConfigurator(IRegistrationConfigurator, ISagaRegistration)**

```csharp
public SagaRegistrationConfigurator(IRegistrationConfigurator configurator, ISagaRegistration registration)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`registration` [ISagaRegistration](../masstransit-configuration/isagaregistration)<br/>

## Methods

### **Endpoint(Action\<IEndpointRegistrationConfigurator\>)**

```csharp
public ISagaRegistrationConfigurator<TSaga> Endpoint(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ISagaRegistrationConfigurator\<TSaga\>](../masstransit/isagaregistrationconfigurator-1)<br/>

### **Repository(Action\<ISagaRepositoryRegistrationConfigurator\<TSaga\>\>)**

```csharp
public ISagaRegistrationConfigurator<TSaga> Repository(Action<ISagaRepositoryRegistrationConfigurator<TSaga>> configure)
```

#### Parameters

`configure` [Action\<ISagaRepositoryRegistrationConfigurator\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ISagaRegistrationConfigurator\<TSaga\>](../masstransit/isagaregistrationconfigurator-1)<br/>
