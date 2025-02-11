---

title: RequestActivityImpl<TInstance, TRequest, TResponse>

---

# RequestActivityImpl\<TInstance, TRequest, TResponse\>

Namespace: MassTransit.SagaStateMachine

```csharp
public abstract class RequestActivityImpl<TInstance, TRequest, TResponse>
```

#### Type Parameters

`TInstance`<br/>

`TRequest`<br/>

`TResponse`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestActivityImpl\<TInstance, TRequest, TResponse\>](../masstransit-sagastatemachine/requestactivityimpl-3)

## Methods

### **SendRequest(BehaviorContext\<TInstance\>, SendTuple\<TRequest\>, Uri)**

```csharp
protected Task SendRequest(BehaviorContext<TInstance> context, SendTuple<TRequest> sendTuple, Uri serviceAddress)
```

#### Parameters

`context` [BehaviorContext\<TInstance\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`sendTuple` [SendTuple\<TRequest\>](../../masstransit-abstractions/masstransit/sendtuple-1)<br/>

`serviceAddress` Uri<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
