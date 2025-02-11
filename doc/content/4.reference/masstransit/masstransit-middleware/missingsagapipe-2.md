---

title: MissingSagaPipe<TSaga, TMessage>

---

# MissingSagaPipe\<TSaga, TMessage\>

Namespace: MassTransit.Middleware

Dispatches a missing saga message to the saga policy, calling Add if necessary

```csharp
public class MissingSagaPipe<TSaga, TMessage> : IPipe<SagaConsumeContext<TSaga, TMessage>>, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MissingSagaPipe\<TSaga, TMessage\>](../masstransit-middleware/missingsagapipe-2)<br/>
Implements [IPipe\<SagaConsumeContext\<TSaga, TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **MissingSagaPipe(SagaRepositoryContext\<TSaga, TMessage\>, IPipe\<SagaConsumeContext\<TSaga, TMessage\>\>)**

```csharp
public MissingSagaPipe(SagaRepositoryContext<TSaga, TMessage> repositoryContext, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
```

#### Parameters

`repositoryContext` [SagaRepositoryContext\<TSaga, TMessage\>](../masstransit-saga/sagarepositorycontext-2)<br/>

`next` [IPipe\<SagaConsumeContext\<TSaga, TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

## Methods

### **Send(SagaConsumeContext\<TSaga, TMessage\>)**

```csharp
public Task Send(SagaConsumeContext<TSaga, TMessage> context)
```

#### Parameters

`context` [SagaConsumeContext\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/sagaconsumecontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
