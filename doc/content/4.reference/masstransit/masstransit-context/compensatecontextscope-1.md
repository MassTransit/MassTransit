---

title: CompensateContextScope<TLog>

---

# CompensateContextScope\<TLog\>

Namespace: MassTransit.Context

```csharp
public class CompensateContextScope<TLog> : CourierContextScope, IPublishEndpoint, IPublishObserverConnector, ConsumeContext, PipeContext, MessageContext, ISendEndpointProvider, ISendObserverConnector, ConsumeContext<RoutingSlip>, CourierContext, CompensateContext<TLog>, CompensateContext
```

#### Type Parameters

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishEndpoint](../../masstransit-abstractions/masstransit-transports/publishendpoint) → [BaseConsumeContext](../masstransit-context/baseconsumecontext) → [ConsumeContextProxy](../masstransit-context/consumecontextproxy) → [ConsumeContextScope](../masstransit-context/consumecontextscope) → [ConsumeContextScope\<RoutingSlip\>](../masstransit-context/consumecontextscope-1) → [CourierContextScope](../masstransit-context/couriercontextscope) → [CompensateContextScope\<TLog\>](../masstransit-context/compensatecontextscope-1)<br/>
Implements [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [MessageContext](../../masstransit-abstractions/masstransit/messagecontext), [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [ConsumeContext\<RoutingSlip\>](../../masstransit-abstractions/masstransit/consumecontext-1), [CourierContext](../../masstransit-abstractions/masstransit/couriercontext), [CompensateContext\<TLog\>](../../masstransit-abstractions/masstransit/compensatecontext-1), [CompensateContext](../../masstransit-abstractions/masstransit/compensatecontext)

## Properties

### **Log**

```csharp
public TLog Log { get; }
```

#### Property Value

TLog<br/>

### **Result**

```csharp
public CompensationResult Result { get; set; }
```

#### Property Value

[CompensationResult](../../masstransit-abstractions/masstransit/compensationresult)<br/>

### **Message**

```csharp
public RoutingSlip Message { get; }
```

#### Property Value

[RoutingSlip](../../masstransit-abstractions/masstransit-courier-contracts/routingslip)<br/>

### **CancellationToken**

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **MessageId**

```csharp
public Nullable<Guid> MessageId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **RequestId**

```csharp
public Nullable<Guid> RequestId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **CorrelationId**

```csharp
public Nullable<Guid> CorrelationId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConversationId**

```csharp
public Nullable<Guid> ConversationId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **InitiatorId**

```csharp
public Nullable<Guid> InitiatorId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ExpirationTime**

```csharp
public Nullable<DateTime> ExpirationTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SourceAddress**

```csharp
public Uri SourceAddress { get; }
```

#### Property Value

Uri<br/>

### **DestinationAddress**

```csharp
public Uri DestinationAddress { get; }
```

#### Property Value

Uri<br/>

### **ResponseAddress**

```csharp
public Uri ResponseAddress { get; }
```

#### Property Value

Uri<br/>

### **FaultAddress**

```csharp
public Uri FaultAddress { get; }
```

#### Property Value

Uri<br/>

### **SentTime**

```csharp
public Nullable<DateTime> SentTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Headers**

```csharp
public Headers Headers { get; }
```

#### Property Value

[Headers](../../masstransit-abstractions/masstransit/headers)<br/>

### **Host**

```csharp
public HostInfo Host { get; }
```

#### Property Value

[HostInfo](../../masstransit-abstractions/masstransit/hostinfo)<br/>

### **ConsumeCompleted**

```csharp
public Task ConsumeCompleted { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SupportedMessageTypes**

```csharp
public IEnumerable<string> SupportedMessageTypes { get; }
```

#### Property Value

[IEnumerable\<String\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **ReceiveContext**

```csharp
public ReceiveContext ReceiveContext { get; protected set; }
```

#### Property Value

[ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

### **SerializerContext**

```csharp
public SerializerContext SerializerContext { get; }
```

#### Property Value

[SerializerContext](../../masstransit-abstractions/masstransit/serializercontext)<br/>

## Constructors

### **CompensateContextScope(CompensateContext\<TLog\>, Object[])**

```csharp
public CompensateContextScope(CompensateContext<TLog> context, Object[] payloads)
```

#### Parameters

`context` [CompensateContext\<TLog\>](../../masstransit-abstractions/masstransit/compensatecontext-1)<br/>

`payloads` [Object[]](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

## Methods

### **CreateActivityContext\<TActivity\>(TActivity)**

```csharp
public CompensateActivityContext<TActivity, TLog> CreateActivityContext<TActivity>(TActivity activity)
```

#### Type Parameters

`TActivity`<br/>

#### Parameters

`activity` TActivity<br/>

#### Returns

[CompensateActivityContext\<TActivity, TLog\>](../../masstransit-abstractions/masstransit/compensateactivitycontext-2)<br/>

### **Compensated()**

```csharp
public CompensationResult Compensated()
```

#### Returns

[CompensationResult](../../masstransit-abstractions/masstransit/compensationresult)<br/>

### **Compensated(Object)**

```csharp
public CompensationResult Compensated(object values)
```

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[CompensationResult](../../masstransit-abstractions/masstransit/compensationresult)<br/>

### **Compensated(IDictionary\<String, Object\>)**

```csharp
public CompensationResult Compensated(IDictionary<string, object> variables)
```

#### Parameters

`variables` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

#### Returns

[CompensationResult](../../masstransit-abstractions/masstransit/compensationresult)<br/>

### **Failed()**

```csharp
public CompensationResult Failed()
```

#### Returns

[CompensationResult](../../masstransit-abstractions/masstransit/compensationresult)<br/>

### **Failed(Exception)**

```csharp
public CompensationResult Failed(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[CompensationResult](../../masstransit-abstractions/masstransit/compensationresult)<br/>
