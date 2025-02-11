---

title: RetryCompensateContext<TLog>

---

# RetryCompensateContext\<TLog\>

Namespace: MassTransit.Context

```csharp
public class RetryCompensateContext<TLog> : CompensateContextScope<TLog>, IPublishEndpoint, IPublishObserverConnector, ConsumeContext, PipeContext, MessageContext, ISendEndpointProvider, ISendObserverConnector, ConsumeContext<RoutingSlip>, CourierContext, CompensateContext<TLog>, CompensateContext, ConsumeRetryContext
```

#### Type Parameters

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishEndpoint](../../masstransit-abstractions/masstransit-transports/publishendpoint) → [BaseConsumeContext](../masstransit-context/baseconsumecontext) → [ConsumeContextProxy](../masstransit-context/consumecontextproxy) → [ConsumeContextScope](../masstransit-context/consumecontextscope) → [ConsumeContextScope\<RoutingSlip\>](../masstransit-context/consumecontextscope-1) → [CourierContextScope](../masstransit-context/couriercontextscope) → [CompensateContextScope\<TLog\>](../masstransit-context/compensatecontextscope-1) → [RetryCompensateContext\<TLog\>](../masstransit-context/retrycompensatecontext-1)<br/>
Implements [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [MessageContext](../../masstransit-abstractions/masstransit/messagecontext), [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [ConsumeContext\<RoutingSlip\>](../../masstransit-abstractions/masstransit/consumecontext-1), [CourierContext](../../masstransit-abstractions/masstransit/couriercontext), [CompensateContext\<TLog\>](../../masstransit-abstractions/masstransit/compensatecontext-1), [CompensateContext](../../masstransit-abstractions/masstransit/compensatecontext), [ConsumeRetryContext](../../masstransit-abstractions/masstransit/consumeretrycontext)

## Properties

### **RetryAttempt**

```csharp
public int RetryAttempt { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **RetryCount**

```csharp
public int RetryCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

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

### **RetryCompensateContext(CompensateContext\<TLog\>, IRetryPolicy, RetryContext)**

```csharp
public RetryCompensateContext(CompensateContext<TLog> context, IRetryPolicy retryPolicy, RetryContext retryContext)
```

#### Parameters

`context` [CompensateContext\<TLog\>](../../masstransit-abstractions/masstransit/compensatecontext-1)<br/>

`retryPolicy` [IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

`retryContext` [RetryContext](../../masstransit-abstractions/masstransit/retrycontext)<br/>

## Methods

### **CreateNext\<TContext\>(RetryContext)**

```csharp
public TContext CreateNext<TContext>(RetryContext retryContext)
```

#### Type Parameters

`TContext`<br/>

#### Parameters

`retryContext` [RetryContext](../../masstransit-abstractions/masstransit/retrycontext)<br/>

#### Returns

TContext<br/>

### **NotifyPendingFaults()**

```csharp
public Task NotifyPendingFaults()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
