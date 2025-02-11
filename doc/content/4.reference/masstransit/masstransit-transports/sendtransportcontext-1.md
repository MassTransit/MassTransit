---

title: SendTransportContext<TContext>

---

# SendTransportContext\<TContext\>

Namespace: MassTransit.Transports

```csharp
public interface SendTransportContext<TContext> : SendTransportContext, PipeContext, ISendObserverConnector, IPipeContextSource<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>

Implements [SendTransportContext](../masstransit-transports/sendtransportcontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IPipeContextSource\<TContext\>](../../masstransit-abstractions/masstransit/ipipecontextsource-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **GetAgentHandles()**

```csharp
IEnumerable<IAgent> GetAgentHandles()
```

#### Returns

[IEnumerable\<IAgent\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **CreateSendContext\<T\>(TContext, T, IPipe\<SendContext\<T\>\>, CancellationToken)**

Create the send

```csharp
Task<SendContext<T>> CreateSendContext<T>(TContext context, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` TContext<br/>
The send transport context, which may be used to create the underlying send context

`message` T<br/>
The message being sent

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>
The developer supplied pipe to configure the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Send\<T\>(TContext, SendContext\<T\>)**

```csharp
Task Send<T>(TContext transportContext, SendContext<T> sendContext)
```

#### Type Parameters

`T`<br/>

#### Parameters

`transportContext` TContext<br/>

`sendContext` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
