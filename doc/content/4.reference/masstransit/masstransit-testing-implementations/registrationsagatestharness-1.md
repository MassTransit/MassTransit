---

title: RegistrationSagaTestHarness<TSaga>

---

# RegistrationSagaTestHarness\<TSaga\>

Namespace: MassTransit.Testing.Implementations

```csharp
public class RegistrationSagaTestHarness<TSaga> : BaseSagaTestHarness<TSaga>, ISagaTestHarness<TSaga>
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseSagaTestHarness\<TSaga\>](../masstransit-testing-implementations/basesagatestharness-1) → [RegistrationSagaTestHarness\<TSaga\>](../masstransit-testing-implementations/registrationsagatestharness-1)<br/>
Implements [ISagaTestHarness\<TSaga\>](../masstransit-testing/isagatestharness-1)

## Properties

### **Consumed**

```csharp
public IReceivedMessageList Consumed { get; }
```

#### Property Value

[IReceivedMessageList](../masstransit-testing/ireceivedmessagelist)<br/>

### **Sagas**

```csharp
public ISagaList<TSaga> Sagas { get; }
```

#### Property Value

[ISagaList\<TSaga\>](../masstransit-testing/isagalist-1)<br/>

### **Created**

```csharp
public ISagaList<TSaga> Created { get; }
```

#### Property Value

[ISagaList\<TSaga\>](../masstransit-testing/isagalist-1)<br/>

## Constructors

### **RegistrationSagaTestHarness(ISagaRepositoryDecoratorRegistration\<TSaga\>, ISagaRepository\<TSaga\>, ILoadSagaRepository\<TSaga\>, IQuerySagaRepository\<TSaga\>)**

```csharp
public RegistrationSagaTestHarness(ISagaRepositoryDecoratorRegistration<TSaga> registration, ISagaRepository<TSaga> repository, ILoadSagaRepository<TSaga> loadRepository, IQuerySagaRepository<TSaga> queryRepository)
```

#### Parameters

`registration` [ISagaRepositoryDecoratorRegistration\<TSaga\>](../masstransit-configuration/isagarepositorydecoratorregistration-1)<br/>

`repository` [ISagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

`loadRepository` [ILoadSagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/iloadsagarepository-1)<br/>

`queryRepository` [IQuerySagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/iquerysagarepository-1)<br/>
