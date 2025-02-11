---

title: SendSagaPipe<TSaga, T>

---

# SendSagaPipe\<TSaga, T\>

Namespace: MassTransit.Middleware

```csharp
public class SendSagaPipe<TSaga, T> : IPipe<SagaRepositoryContext<TSaga, T>>, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendSagaPipe\<TSaga, T\>](../masstransit-middleware/sendsagapipe-2)<br/>
Implements [IPipe\<SagaRepositoryContext\<TSaga, T\>\>](../../masstransit-abstractions/masstransit/ipipe-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **SendSagaPipe(ISagaPolicy\<TSaga, T\>, IPipe\<SagaConsumeContext\<TSaga, T\>\>, Guid)**

```csharp
public SendSagaPipe(ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next, Guid correlationId)
```

#### Parameters

`policy` [ISagaPolicy\<TSaga, T\>](../../masstransit-abstractions/masstransit/isagapolicy-2)<br/>

`next` [IPipe\<SagaConsumeContext\<TSaga, T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Send(SagaRepositoryContext\<TSaga, T\>)**

```csharp
public Task Send(SagaRepositoryContext<TSaga, T> context)
```

#### Parameters

`context` [SagaRepositoryContext\<TSaga, T\>](../masstransit-saga/sagarepositorycontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
