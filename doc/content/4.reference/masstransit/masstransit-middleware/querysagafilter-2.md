---

title: QuerySagaFilter<TSaga, TMessage>

---

# QuerySagaFilter\<TSaga, TMessage\>

Namespace: MassTransit.Middleware

Creates a filter to send a query to the saga repository using the query factory and saga policy provided.

```csharp
public class QuerySagaFilter<TSaga, TMessage> : IFilter<ConsumeContext<TMessage>>, IProbeSite
```

#### Type Parameters

`TSaga`<br/>
The saga type

`TMessage`<br/>
The message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [QuerySagaFilter\<TSaga, TMessage\>](../masstransit-middleware/querysagafilter-2)<br/>
Implements [IFilter\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **QuerySagaFilter(ISagaRepository\<TSaga\>, ISagaPolicy\<TSaga, TMessage\>, ISagaQueryFactory\<TSaga, TMessage\>, IPipe\<SagaConsumeContext\<TSaga, TMessage\>\>)**

```csharp
public QuerySagaFilter(ISagaRepository<TSaga> sagaRepository, ISagaPolicy<TSaga, TMessage> policy, ISagaQueryFactory<TSaga, TMessage> queryFactory, IPipe<SagaConsumeContext<TSaga, TMessage>> messagePipe)
```

#### Parameters

`sagaRepository` [ISagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

`policy` [ISagaPolicy\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/isagapolicy-2)<br/>

`queryFactory` [ISagaQueryFactory\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/isagaqueryfactory-2)<br/>

`messagePipe` [IPipe\<SagaConsumeContext\<TSaga, TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Send(ConsumeContext\<TMessage\>, IPipe\<ConsumeContext\<TMessage\>\>)**

```csharp
public Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`next` [IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
