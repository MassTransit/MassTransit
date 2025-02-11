---

title: RequestStateMessagePipe

---

# RequestStateMessagePipe

Namespace: MassTransit.SagaStateMachine

```csharp
public class RequestStateMessagePipe : IPipe<SendContext>, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestStateMessagePipe](../masstransit-sagastatemachine/requeststatemessagepipe)<br/>
Implements [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **RequestStateMessagePipe(BehaviorContext\<RequestState\>, Object, String[])**

```csharp
public RequestStateMessagePipe(BehaviorContext<RequestState> context, object message, String[] messageType)
```

#### Parameters

`context` [BehaviorContext\<RequestState\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Send(SendContext)**

```csharp
public Task Send(SendContext context)
```

#### Parameters

`context` [SendContext](../../masstransit-abstractions/masstransit/sendcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
