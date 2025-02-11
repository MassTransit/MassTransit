---

title: ConsumeContextOutputMessageTypeFilter<TMessage>

---

# ConsumeContextOutputMessageTypeFilter\<TMessage\>

Namespace: MassTransit.Middleware

Converts an inbound context type to a pipe context type post-dispatch

```csharp
public class ConsumeContextOutputMessageTypeFilter<TMessage> : IConsumeContextOutputMessageTypeFilter<TMessage>, IFilter<ConsumeContext>, IProbeSite, IPipeConnector<ConsumeContext<TMessage>>, IConsumeMessageObserverConnector<TMessage>
```

#### Type Parameters

`TMessage`<br/>
The subsequent pipe context type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumeContextOutputMessageTypeFilter\<TMessage\>](../masstransit-middleware/consumecontextoutputmessagetypefilter-1)<br/>
Implements [IConsumeContextOutputMessageTypeFilter\<TMessage\>](../masstransit-middleware/iconsumecontextoutputmessagetypefilter-1), [IFilter\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IPipeConnector\<ConsumeContext\<TMessage\>\>](../masstransit-middleware/ipipeconnector-1), [IConsumeMessageObserverConnector\<TMessage\>](../../masstransit-abstractions/masstransit/iconsumemessageobserverconnector-1)

## Constructors

### **ConsumeContextOutputMessageTypeFilter(ConsumeObservable, IRequestIdTeeFilter\<TMessage\>)**

```csharp
public ConsumeContextOutputMessageTypeFilter(ConsumeObservable observers, IRequestIdTeeFilter<TMessage> output)
```

#### Parameters

`observers` [ConsumeObservable](../../masstransit-abstractions/masstransit-observables/consumeobservable)<br/>

`output` [IRequestIdTeeFilter\<TMessage\>](../masstransit-middleware/irequestidteefilter-1)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Send(ConsumeContext, IPipe\<ConsumeContext\>)**

```csharp
public Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
```

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`next` [IPipe\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ConnectConsumeMessageObserver(IConsumeMessageObserver\<TMessage\>)**

```csharp
public ConnectHandle ConnectConsumeMessageObserver(IConsumeMessageObserver<TMessage> observer)
```

#### Parameters

`observer` [IConsumeMessageObserver\<TMessage\>](../../masstransit-abstractions/masstransit/iconsumemessageobserver-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectPipe(IPipe\<ConsumeContext\<TMessage\>\>)**

```csharp
public ConnectHandle ConnectPipe(IPipe<ConsumeContext<TMessage>> pipe)
```

#### Parameters

`pipe` [IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectPipe(Guid, IPipe\<ConsumeContext\<TMessage\>\>)**

```csharp
public ConnectHandle ConnectPipe(Guid key, IPipe<ConsumeContext<TMessage>> pipe)
```

#### Parameters

`key` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`pipe` [IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
