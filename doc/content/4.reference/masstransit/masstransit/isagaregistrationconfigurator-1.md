---

title: ISagaRegistrationConfigurator<TSaga>

---

# ISagaRegistrationConfigurator\<TSaga\>

Namespace: MassTransit

```csharp
public interface ISagaRegistrationConfigurator<TSaga> : ISagaRegistrationConfigurator
```

#### Type Parameters

`TSaga`<br/>

Implements [ISagaRegistrationConfigurator](../masstransit/isagaregistrationconfigurator)

## Methods

### **Endpoint(Action\<IEndpointRegistrationConfigurator\>)**

```csharp
ISagaRegistrationConfigurator<TSaga> Endpoint(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ISagaRegistrationConfigurator\<TSaga\>](../masstransit/isagaregistrationconfigurator-1)<br/>

### **Repository(Action\<ISagaRepositoryRegistrationConfigurator\<TSaga\>\>)**

```csharp
ISagaRegistrationConfigurator<TSaga> Repository(Action<ISagaRepositoryRegistrationConfigurator<TSaga>> configure)
```

#### Parameters

`configure` [Action\<ISagaRepositoryRegistrationConfigurator\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ISagaRegistrationConfigurator\<TSaga\>](../masstransit/isagaregistrationconfigurator-1)<br/>
