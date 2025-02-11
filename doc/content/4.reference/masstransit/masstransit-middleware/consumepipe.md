---

title: ConsumePipe

---

# ConsumePipe

Namespace: MassTransit.Middleware

```csharp
public class ConsumePipe : IConsumePipe, IPipe<ConsumeContext>, IProbeSite, IConsumePipeConnector, IRequestPipeConnector, IConsumeMessageObserverConnector, IConsumeObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumePipe](../masstransit-middleware/consumepipe)<br/>
Implements [IConsumePipe](../../masstransit-abstractions/masstransit-transports/iconsumepipe), [IPipe\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipe-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector), [IRequestPipeConnector](../../masstransit-abstractions/masstransit/irequestpipeconnector), [IConsumeMessageObserverConnector](../../masstransit-abstractions/masstransit/iconsumemessageobserverconnector), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector)

## Properties

### **Connected**

```csharp
public Task Connected { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

## Constructors

### **ConsumePipe(IConsumePipeSpecification, IConsumeContextMessageTypeFilter, IPipe\<ConsumeContext\>, Boolean)**

```csharp
public ConsumePipe(IConsumePipeSpecification specification, IConsumeContextMessageTypeFilter filter, IPipe<ConsumeContext> pipe, bool autoStart)
```

#### Parameters

`specification` [IConsumePipeSpecification](../../masstransit-abstractions/masstransit-configuration/iconsumepipespecification)<br/>

`filter` [IConsumeContextMessageTypeFilter](../masstransit-middleware/iconsumecontextmessagetypefilter)<br/>

`pipe` [IPipe\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`autoStart` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Send(ConsumeContext)**

```csharp
public Task Send(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ConnectConsumeMessageObserver\<TMessage\>(IConsumeMessageObserver\<TMessage\>)**

```csharp
public ConnectHandle ConnectConsumeMessageObserver<TMessage>(IConsumeMessageObserver<TMessage> observer)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`observer` [IConsumeMessageObserver\<TMessage\>](../../masstransit-abstractions/masstransit/iconsumemessageobserver-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectConsumePipe\<T\>(IPipe\<ConsumeContext\<T\>\>)**

```csharp
public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipe` [IPipe\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectConsumePipe\<T\>(IPipe\<ConsumeContext\<T\>\>, ConnectPipeOptions)**

```csharp
public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipe` [IPipe\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`options` [ConnectPipeOptions](../../masstransit-abstractions/masstransit/connectpipeoptions)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectRequestPipe\<T\>(Guid, IPipe\<ConsumeContext\<T\>\>)**

```csharp
public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`requestId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`pipe` [IPipe\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

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
