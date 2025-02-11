---

title: SagaExtensions

---

# SagaExtensions

Namespace: MassTransit

```csharp
public static class SagaExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaExtensions](../masstransit/sagaextensions)

## Methods

### **Saga\<T\>(IReceiveEndpointConfigurator, ISagaRepository\<T\>, Action\<ISagaConfigurator\<T\>\>)**

Configure a saga subscription

```csharp
public static void Saga<T>(IReceiveEndpointConfigurator configurator, ISagaRepository<T> sagaRepository, Action<ISagaConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`sagaRepository` [ISagaRepository\<T\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

`configure` [Action\<ISagaConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConnectSaga\<T\>(IConsumePipeConnector, ISagaRepository\<T\>, IPipeSpecification`1[])**

Connects the saga to the bus

```csharp
public static ConnectHandle ConnectSaga<T>(IConsumePipeConnector connector, ISagaRepository<T> sagaRepository, IPipeSpecification`1[] pipeSpecifications)
```

#### Type Parameters

`T`<br/>
The saga type

#### Parameters

`connector` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>
The bus to which the saga is to be connected

`sagaRepository` [ISagaRepository\<T\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>
The saga repository

`pipeSpecifications` [IPipeSpecification`1[]](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
