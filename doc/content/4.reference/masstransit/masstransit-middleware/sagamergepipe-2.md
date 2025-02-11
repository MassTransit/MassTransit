---

title: SagaMergePipe<TSaga, TMessage>

---

# SagaMergePipe\<TSaga, TMessage\>

Namespace: MassTransit.Middleware

Merges the out-of-band message back into the pipe

```csharp
public class SagaMergePipe<TSaga, TMessage> : IPipe<SagaConsumeContext<TSaga>>, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaMergePipe\<TSaga, TMessage\>](../masstransit-middleware/sagamergepipe-2)<br/>
Implements [IPipe\<SagaConsumeContext\<TSaga\>\>](../../masstransit-abstractions/masstransit/ipipe-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **SagaMergePipe(IPipe\<SagaConsumeContext\<TSaga, TMessage\>\>)**

```csharp
public SagaMergePipe(IPipe<SagaConsumeContext<TSaga, TMessage>> output)
```

#### Parameters

`output` [IPipe\<SagaConsumeContext\<TSaga, TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

## Methods

### **Send(SagaConsumeContext\<TSaga\>)**

```csharp
public Task Send(SagaConsumeContext<TSaga> context)
```

#### Parameters

`context` [SagaConsumeContext\<TSaga\>](../../masstransit-abstractions/masstransit/sagaconsumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
