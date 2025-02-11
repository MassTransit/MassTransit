---

title: SagaRepositoryContext<TSaga, TMessage>

---

# SagaRepositoryContext\<TSaga, TMessage\>

Namespace: MassTransit.Saga

```csharp
public interface SagaRepositoryContext<TSaga, TMessage> : ISagaConsumeContextFactory<TSaga>, ConsumeContext<TMessage>, ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Implements [ISagaConsumeContextFactory\<TSaga\>](../masstransit-saga/isagaconsumecontextfactory-1), [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1), [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [MessageContext](../../masstransit-abstractions/masstransit/messagecontext), [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector)

## Methods

### **Add(TSaga)**

Add the saga instance, using the specified

```csharp
Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
```

#### Parameters

`instance` TSaga<br/>

#### Returns

[Task\<SagaConsumeContext\<TSaga, TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Insert(TSaga)**

Insert the saga instance, if it does not already exist.

```csharp
Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
```

#### Parameters

`instance` TSaga<br/>

#### Returns

[Task\<SagaConsumeContext\<TSaga, TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
A valid  if the instance inserted successfully, otherwise default

### **Load(Guid)**

Load an existing saga instance

```csharp
Task<SagaConsumeContext<TSaga, TMessage>> Load(Guid correlationId)
```

#### Parameters

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Task\<SagaConsumeContext\<TSaga, TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
A valid  if the instance loaded successfully, otherwise default

### **Save(SagaConsumeContext\<TSaga\>)**

Save the saga, called after an Add, without an insert

```csharp
Task Save(SagaConsumeContext<TSaga> context)
```

#### Parameters

`context` [SagaConsumeContext\<TSaga\>](../../masstransit-abstractions/masstransit/sagaconsumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Update(SagaConsumeContext\<TSaga\>)**

Update the saga, called after a load or insert where the saga has not completed

```csharp
Task Update(SagaConsumeContext<TSaga> context)
```

#### Parameters

`context` [SagaConsumeContext\<TSaga\>](../../masstransit-abstractions/masstransit/sagaconsumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Delete(SagaConsumeContext\<TSaga\>)**

Delete the saga, called after a Load when the saga is completed

```csharp
Task Delete(SagaConsumeContext<TSaga> context)
```

#### Parameters

`context` [SagaConsumeContext\<TSaga\>](../../masstransit-abstractions/masstransit/sagaconsumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Discard(SagaConsumeContext\<TSaga\>)**

Discard the saga, called after an Add when the saga is completed within the same transaction

```csharp
Task Discard(SagaConsumeContext<TSaga> context)
```

#### Parameters

`context` [SagaConsumeContext\<TSaga\>](../../masstransit-abstractions/masstransit/sagaconsumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Undo(SagaConsumeContext\<TSaga\>)**

Undo the changes for the saga

```csharp
Task Undo(SagaConsumeContext<TSaga> context)
```

#### Parameters

`context` [SagaConsumeContext\<TSaga\>](../../masstransit-abstractions/masstransit/sagaconsumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
