---

title: SagaContainerTestHarnessRegistration<TSaga>

---

# SagaContainerTestHarnessRegistration\<TSaga\>

Namespace: MassTransit.DependencyInjection.Testing

```csharp
public class SagaContainerTestHarnessRegistration<TSaga> : ISagaRepositoryDecoratorRegistration<TSaga>
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaContainerTestHarnessRegistration\<TSaga\>](../masstransit-dependencyinjection-testing/sagacontainertestharnessregistration-1)<br/>
Implements [ISagaRepositoryDecoratorRegistration\<TSaga\>](../masstransit-configuration/isagarepositorydecoratorregistration-1)

## Properties

### **TestTimeout**

```csharp
public TimeSpan TestTimeout { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **Consumed**

```csharp
public ReceivedMessageList Consumed { get; }
```

#### Property Value

[ReceivedMessageList](../masstransit-testing/receivedmessagelist)<br/>

### **Created**

```csharp
public SagaList<TSaga> Created { get; }
```

#### Property Value

[SagaList\<TSaga\>](../masstransit-testing-implementations/sagalist-1)<br/>

### **Sagas**

```csharp
public SagaList<TSaga> Sagas { get; }
```

#### Property Value

[SagaList\<TSaga\>](../masstransit-testing-implementations/sagalist-1)<br/>

## Constructors

### **SagaContainerTestHarnessRegistration(ITestHarness)**

```csharp
public SagaContainerTestHarnessRegistration(ITestHarness testHarness)
```

#### Parameters

`testHarness` [ITestHarness](../masstransit-testing/itestharness)<br/>

## Methods

### **DecorateSagaRepository(ISagaRepository\<TSaga\>)**

```csharp
public ISagaRepository<TSaga> DecorateSagaRepository(ISagaRepository<TSaga> repository)
```

#### Parameters

`repository` [ISagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

#### Returns

[ISagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>
