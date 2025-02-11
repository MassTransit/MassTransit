---

title: OutboxConsumeContext

---

# OutboxConsumeContext

Namespace: MassTransit.Middleware

```csharp
public interface OutboxConsumeContext : ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector, OutboxSendContext, IServiceProvider
```

Implements [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [MessageContext](../../masstransit-abstractions/masstransit/messagecontext), [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [OutboxSendContext](../masstransit-middleware/outboxsendcontext), IServiceProvider

## Properties

### **CapturedContext**

```csharp
public abstract ConsumeContext CapturedContext { get; }
```

#### Property Value

[ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

### **ContinueProcessing**

If true, continue processing

```csharp
public abstract bool ContinueProcessing { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsMessageConsumed**

If true, the message was already consumed

```csharp
public abstract bool IsMessageConsumed { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsOutboxDelivered**

If true, the outbox messages have already been dispatched

```csharp
public abstract bool IsOutboxDelivered { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ReceiveCount**

The number of delivery attempts for this message

```csharp
public abstract int ReceiveCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **LastSequenceNumber**

The last sequence number produced from the outbox

```csharp
public abstract Nullable<long> LastSequenceNumber { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Methods

### **SetConsumed()**

```csharp
Task SetConsumed()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SetDelivered()**

```csharp
Task SetDelivered()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **LoadOutboxMessages()**

```csharp
Task<List<OutboxMessageContext>> LoadOutboxMessages()
```

#### Returns

[Task\<List\<OutboxMessageContext\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **NotifyOutboxMessageDelivered(OutboxMessageContext)**

```csharp
Task NotifyOutboxMessageDelivered(OutboxMessageContext message)
```

#### Parameters

`message` [OutboxMessageContext](../masstransit-middleware/outboxmessagecontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RemoveOutboxMessages()**

```csharp
Task RemoveOutboxMessages()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
