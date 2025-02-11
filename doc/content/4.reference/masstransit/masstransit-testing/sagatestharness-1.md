---

title: SagaTestHarness<TSaga>

---

# SagaTestHarness\<TSaga\>

Namespace: MassTransit.Testing

```csharp
public class SagaTestHarness<TSaga> : BaseSagaTestHarness<TSaga>, ISagaTestHarness<TSaga>
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseSagaTestHarness\<TSaga\>](../masstransit-testing-implementations/basesagatestharness-1) → [SagaTestHarness\<TSaga\>](../masstransit-testing/sagatestharness-1)<br/>
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

### **SagaTestHarness(BusTestHarness, ISagaRepository\<TSaga\>, IQuerySagaRepository\<TSaga\>, ILoadSagaRepository\<TSaga\>, String)**

```csharp
public SagaTestHarness(BusTestHarness testHarness, ISagaRepository<TSaga> repository, IQuerySagaRepository<TSaga> querySagaRepository, ILoadSagaRepository<TSaga> loadSagaRepository, string queueName)
```

#### Parameters

`testHarness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

`repository` [ISagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

`querySagaRepository` [IQuerySagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/iquerysagarepository-1)<br/>

`loadSagaRepository` [ILoadSagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/iloadsagarepository-1)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **ConfigureReceiveEndpoint(IReceiveEndpointConfigurator)**

```csharp
protected void ConfigureReceiveEndpoint(IReceiveEndpointConfigurator configurator)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

### **ConfigureNamedReceiveEndpoint(IBusFactoryConfigurator, String)**

```csharp
protected void ConfigureNamedReceiveEndpoint(IBusFactoryConfigurator configurator, string queueName)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
