---

title: SendQuerySagaPipe<TSaga, T>

---

# SendQuerySagaPipe\<TSaga, T\>

Namespace: MassTransit.Middleware

```csharp
public class SendQuerySagaPipe<TSaga, T> : IPipe<SagaRepositoryQueryContext<TSaga, T>>, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendQuerySagaPipe\<TSaga, T\>](../masstransit-middleware/sendquerysagapipe-2)<br/>
Implements [IPipe\<SagaRepositoryQueryContext\<TSaga, T\>\>](../../masstransit-abstractions/masstransit/ipipe-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **SendQuerySagaPipe(ISagaPolicy\<TSaga, T\>, IPipe\<SagaConsumeContext\<TSaga, T\>\>)**

```csharp
public SendQuerySagaPipe(ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
```

#### Parameters

`policy` [ISagaPolicy\<TSaga, T\>](../../masstransit-abstractions/masstransit/isagapolicy-2)<br/>

`next` [IPipe\<SagaConsumeContext\<TSaga, T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Send(SagaRepositoryQueryContext\<TSaga, T\>)**

```csharp
public Task Send(SagaRepositoryQueryContext<TSaga, T> context)
```

#### Parameters

`context` [SagaRepositoryQueryContext\<TSaga, T\>](../masstransit-saga/sagarepositoryquerycontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
