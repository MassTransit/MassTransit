---

title: SagaTestHarnessRegistration<TSaga>

---

# SagaTestHarnessRegistration\<TSaga\>

Namespace: MassTransit.DependencyInjection.Testing

```csharp
public class SagaTestHarnessRegistration<TSaga> : ISagaRepositoryDecoratorRegistration<TSaga>
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaTestHarnessRegistration\<TSaga\>](../masstransit-dependencyinjection-testing/sagatestharnessregistration-1)<br/>
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

### **SagaTestHarnessRegistration(BusTestHarness)**

```csharp
public SagaTestHarnessRegistration(BusTestHarness testHarness)
```

#### Parameters

`testHarness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

## Methods

### **DecorateSagaRepository(ISagaRepository\<TSaga\>)**

```csharp
public ISagaRepository<TSaga> DecorateSagaRepository(ISagaRepository<TSaga> repository)
```

#### Parameters

`repository` [ISagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

#### Returns

[ISagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>
