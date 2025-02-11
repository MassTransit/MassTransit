---

title: SagaRepositoryRegistrationProvider

---

# SagaRepositoryRegistrationProvider

Namespace: MassTransit.Configuration

```csharp
public class SagaRepositoryRegistrationProvider : ISagaRepositoryRegistrationProvider
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaRepositoryRegistrationProvider](../masstransit-configuration/sagarepositoryregistrationprovider)<br/>
Implements [ISagaRepositoryRegistrationProvider](../masstransit-configuration/isagarepositoryregistrationprovider)

## Constructors

### **SagaRepositoryRegistrationProvider()**

```csharp
public SagaRepositoryRegistrationProvider()
```

## Methods

### **Configure\<TSaga\>(ISagaRegistrationConfigurator\<TSaga\>)**

```csharp
public void Configure<TSaga>(ISagaRegistrationConfigurator<TSaga> configurator)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`configurator` [ISagaRegistrationConfigurator\<TSaga\>](../masstransit/isagaregistrationconfigurator-1)<br/>
