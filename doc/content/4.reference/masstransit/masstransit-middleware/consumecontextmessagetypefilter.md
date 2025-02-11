---

title: ConsumeContextMessageTypeFilter

---

# ConsumeContextMessageTypeFilter

Namespace: MassTransit.Middleware

Converts ConsumeContext to ConsumeContext&lt;T&gt; for a given message type
 type.

```csharp
public class ConsumeContextMessageTypeFilter : IConsumeContextMessageTypeFilter, IFilter<ConsumeContext>, IProbeSite, IConsumeMessageObserverConnector, IConsumeObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumeContextMessageTypeFilter](../masstransit-middleware/consumecontextmessagetypefilter)<br/>
Implements [IConsumeContextMessageTypeFilter](../masstransit-middleware/iconsumecontextmessagetypefilter), [IFilter\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IConsumeMessageObserverConnector](../../masstransit-abstractions/masstransit/iconsumemessageobserverconnector), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector)

## Constructors

### **ConsumeContextMessageTypeFilter()**

```csharp
public ConsumeContextMessageTypeFilter()
```

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **ConnectMessagePipe\<T\>(IPipe\<ConsumeContext\<T\>\>)**

```csharp
public ConnectHandle ConnectMessagePipe<T>(IPipe<ConsumeContext<T>> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipe` [IPipe\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectMessagePipe\<T\>(Guid, IPipe\<ConsumeContext\<T\>\>)**

```csharp
public ConnectHandle ConnectMessagePipe<T>(Guid key, IPipe<ConsumeContext<T>> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`key` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`pipe` [IPipe\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **Send(ConsumeContext, IPipe\<ConsumeContext\>)**

```csharp
public Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
```

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`next` [IPipe\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ConnectConsumeMessageObserver\<T\>(IConsumeMessageObserver\<T\>)**

```csharp
public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
```

#### Type Parameters

`T`<br/>

#### Parameters

`observer` [IConsumeMessageObserver\<T\>](../../masstransit-abstractions/masstransit/iconsumemessageobserver-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectConsumeObserver(IConsumeObserver)**

```csharp
public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
```

#### Parameters

`observer` [IConsumeObserver](../../masstransit-abstractions/masstransit/iconsumeobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
