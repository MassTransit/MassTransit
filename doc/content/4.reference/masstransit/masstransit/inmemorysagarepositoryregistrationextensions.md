---

title: InMemorySagaRepositoryRegistrationExtensions

---

# InMemorySagaRepositoryRegistrationExtensions

Namespace: MassTransit

```csharp
public static class InMemorySagaRepositoryRegistrationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemorySagaRepositoryRegistrationExtensions](../masstransit/inmemorysagarepositoryregistrationextensions)

## Methods

### **InMemoryRepository\<T\>(ISagaRegistrationConfigurator\<T\>)**

Adds an in-memory saga repository to the registration

```csharp
public static ISagaRegistrationConfigurator<T> InMemoryRepository<T>(ISagaRegistrationConfigurator<T> configurator)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [ISagaRegistrationConfigurator\<T\>](../masstransit/isagaregistrationconfigurator-1)<br/>

#### Returns

[ISagaRegistrationConfigurator\<T\>](../masstransit/isagaregistrationconfigurator-1)<br/>

### **SetInMemorySagaRepositoryProvider(IRegistrationConfigurator)**

Use the InMemorySagaRepository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)

```csharp
public static void SetInMemorySagaRepositoryProvider(IRegistrationConfigurator configurator)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>
