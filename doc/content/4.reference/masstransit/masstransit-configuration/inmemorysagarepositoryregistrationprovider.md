---

title: InMemorySagaRepositoryRegistrationProvider

---

# InMemorySagaRepositoryRegistrationProvider

Namespace: MassTransit.Configuration

```csharp
public class InMemorySagaRepositoryRegistrationProvider : ISagaRepositoryRegistrationProvider
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemorySagaRepositoryRegistrationProvider](../masstransit-configuration/inmemorysagarepositoryregistrationprovider)<br/>
Implements [ISagaRepositoryRegistrationProvider](../masstransit-configuration/isagarepositoryregistrationprovider)

## Constructors

### **InMemorySagaRepositoryRegistrationProvider()**

```csharp
public InMemorySagaRepositoryRegistrationProvider()
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
