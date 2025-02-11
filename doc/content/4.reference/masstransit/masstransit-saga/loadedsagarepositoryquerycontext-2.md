---

title: LoadedSagaRepositoryQueryContext<TSaga, TMessage>

---

# LoadedSagaRepositoryQueryContext\<TSaga, TMessage\>

Namespace: MassTransit.Saga

For queries that load the actual saga instances

```csharp
public class LoadedSagaRepositoryQueryContext<TSaga, TMessage> : ConsumeContextProxy<TMessage>, IPublishEndpoint, IPublishObserverConnector, ConsumeContext, PipeContext, MessageContext, ISendEndpointProvider, ISendObserverConnector, ConsumeContext<TMessage>, SagaRepositoryQueryContext<TSaga, TMessage>, SagaRepositoryContext<TSaga, TMessage>, ISagaConsumeContextFactory<TSaga>, IEnumerable<Guid>, IEnumerable
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishEndpoint](../../masstransit-abstractions/masstransit-transports/publishendpoint) → [BaseConsumeContext](../masstransit-context/baseconsumecontext) → [ConsumeContextProxy](../masstransit-context/consumecontextproxy) → [ConsumeContextProxy\<TMessage\>](../masstransit-context/consumecontextproxy-1) → [LoadedSagaRepositoryQueryContext\<TSaga, TMessage\>](../masstransit-saga/loadedsagarepositoryquerycontext-2)<br/>
Implements [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [MessageContext](../../masstransit-abstractions/masstransit/messagecontext), [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1), [SagaRepositoryQueryContext\<TSaga, TMessage\>](../masstransit-saga/sagarepositoryquerycontext-2), [SagaRepositoryContext\<TSaga, TMessage\>](../masstransit-saga/sagarepositorycontext-2), [ISagaConsumeContextFactory\<TSaga\>](../masstransit-saga/isagaconsumecontextfactory-1), [IEnumerable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Message**

```csharp
public TMessage Message { get; }
```

#### Property Value

TMessage<br/>

### **CancellationToken**

Returns the CancellationToken for the context (implicit interface)

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **MessageId**

```csharp
public Nullable<Guid> MessageId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **RequestId**

```csharp
public Nullable<Guid> RequestId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **CorrelationId**

```csharp
public Nullable<Guid> CorrelationId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConversationId**

```csharp
public Nullable<Guid> ConversationId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **InitiatorId**

```csharp
public Nullable<Guid> InitiatorId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ExpirationTime**

```csharp
public Nullable<DateTime> ExpirationTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SourceAddress**

```csharp
public Uri SourceAddress { get; }
```

#### Property Value

Uri<br/>

### **DestinationAddress**

```csharp
public Uri DestinationAddress { get; }
```

#### Property Value

Uri<br/>

### **ResponseAddress**

```csharp
public Uri ResponseAddress { get; }
```

#### Property Value

Uri<br/>

### **FaultAddress**

```csharp
public Uri FaultAddress { get; }
```

#### Property Value

Uri<br/>

### **SentTime**

```csharp
public Nullable<DateTime> SentTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Headers**

```csharp
public Headers Headers { get; }
```

#### Property Value

[Headers](../../masstransit-abstractions/masstransit/headers)<br/>

### **Host**

```csharp
public HostInfo Host { get; }
```

#### Property Value

[HostInfo](../../masstransit-abstractions/masstransit/hostinfo)<br/>

### **ConsumeCompleted**

```csharp
public Task ConsumeCompleted { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SupportedMessageTypes**

```csharp
public IEnumerable<string> SupportedMessageTypes { get; }
```

#### Property Value

[IEnumerable\<String\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **ReceiveContext**

```csharp
public ReceiveContext ReceiveContext { get; protected set; }
```

#### Property Value

[ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

### **SerializerContext**

```csharp
public SerializerContext SerializerContext { get; }
```

#### Property Value

[SerializerContext](../../masstransit-abstractions/masstransit/serializercontext)<br/>

## Constructors

### **LoadedSagaRepositoryQueryContext(SagaRepositoryContext\<TSaga, TMessage\>, IEnumerable\<TSaga\>)**

```csharp
public LoadedSagaRepositoryQueryContext(SagaRepositoryContext<TSaga, TMessage> repositoryContext, IEnumerable<TSaga> instances)
```

#### Parameters

`repositoryContext` [SagaRepositoryContext\<TSaga, TMessage\>](../masstransit-saga/sagarepositorycontext-2)<br/>

`instances` [IEnumerable\<TSaga\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Methods

### **Add(TSaga)**

```csharp
public Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
```

#### Parameters

`instance` TSaga<br/>

#### Returns

[Task\<SagaConsumeContext\<TSaga, TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Insert(TSaga)**

```csharp
public Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
```

#### Parameters

`instance` TSaga<br/>

#### Returns

[Task\<SagaConsumeContext\<TSaga, TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Load(Guid)**

```csharp
public Task<SagaConsumeContext<TSaga, TMessage>> Load(Guid correlationId)
```

#### Parameters

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Task\<SagaConsumeContext\<TSaga, TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Save(SagaConsumeContext\<TSaga\>)**

```csharp
public Task Save(SagaConsumeContext<TSaga> context)
```

#### Parameters

`context` [SagaConsumeContext\<TSaga\>](../../masstransit-abstractions/masstransit/sagaconsumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Discard(SagaConsumeContext\<TSaga\>)**

```csharp
public Task Discard(SagaConsumeContext<TSaga> context)
```

#### Parameters

`context` [SagaConsumeContext\<TSaga\>](../../masstransit-abstractions/masstransit/sagaconsumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Undo(SagaConsumeContext\<TSaga\>)**

```csharp
public Task Undo(SagaConsumeContext<TSaga> context)
```

#### Parameters

`context` [SagaConsumeContext\<TSaga\>](../../masstransit-abstractions/masstransit/sagaconsumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Update(SagaConsumeContext\<TSaga\>)**

```csharp
public Task Update(SagaConsumeContext<TSaga> context)
```

#### Parameters

`context` [SagaConsumeContext\<TSaga\>](../../masstransit-abstractions/masstransit/sagaconsumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Delete(SagaConsumeContext\<TSaga\>)**

```csharp
public Task Delete(SagaConsumeContext<TSaga> context)
```

#### Parameters

`context` [SagaConsumeContext\<TSaga\>](../../masstransit-abstractions/masstransit/sagaconsumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **GetEnumerator()**

```csharp
public IEnumerator<Guid> GetEnumerator()
```

#### Returns

[IEnumerator\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1)<br/>

### **CreateSagaConsumeContext\<T\>(ConsumeContext\<T\>, TSaga, SagaConsumeContextMode)**

```csharp
public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(ConsumeContext<T> consumeContext, TSaga instance, SagaConsumeContextMode mode)
```

#### Type Parameters

`T`<br/>

#### Parameters

`consumeContext` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`instance` TSaga<br/>

`mode` [SagaConsumeContextMode](../masstransit-saga/sagaconsumecontextmode)<br/>

#### Returns

[Task\<SagaConsumeContext\<TSaga, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
