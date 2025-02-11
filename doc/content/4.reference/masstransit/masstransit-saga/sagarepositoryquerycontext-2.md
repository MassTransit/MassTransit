---

title: SagaRepositoryQueryContext<TSaga, T>

---

# SagaRepositoryQueryContext\<TSaga, T\>

Namespace: MassTransit.Saga

```csharp
public interface SagaRepositoryQueryContext<TSaga, T> : SagaRepositoryContext<TSaga, T>, ISagaConsumeContextFactory<TSaga>, ConsumeContext<T>, ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector, IEnumerable<Guid>, IEnumerable
```

#### Type Parameters

`TSaga`<br/>

`T`<br/>

Implements [SagaRepositoryContext\<TSaga, T\>](../masstransit-saga/sagarepositorycontext-2), [ISagaConsumeContextFactory\<TSaga\>](../masstransit-saga/isagaconsumecontextfactory-1), [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1), [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [MessageContext](../../masstransit-abstractions/masstransit/messagecontext), [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IEnumerable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **Count**

The number of matching saga instances

```csharp
public abstract int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
