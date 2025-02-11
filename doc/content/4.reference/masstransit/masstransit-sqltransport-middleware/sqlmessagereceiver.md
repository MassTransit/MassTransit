---

title: SqlMessageReceiver

---

# SqlMessageReceiver

Namespace: MassTransit.SqlTransport.Middleware

Receives messages from AmazonSQS, pushing them to the InboundPipe of the service endpoint.

```csharp
public sealed class SqlMessageReceiver : ConsumerAgent<Guid>, IAgent, DeliveryMetrics
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Agent](../../masstransit-abstractions/masstransit-middleware/agent) → [ConsumerAgent\<Guid\>](../masstransit-transports/consumeragent-1) → [SqlMessageReceiver](../masstransit-sqltransport-middleware/sqlmessagereceiver)<br/>
Implements [IAgent](../../masstransit-abstractions/masstransit/iagent), [DeliveryMetrics](../masstransit-transports/deliverymetrics)

## Properties

### **DeliveryCount**

```csharp
public long DeliveryCount { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **ConcurrentDeliveryCount**

```csharp
public int ConcurrentDeliveryCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Ready**

```csharp
public Task Ready { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Completed**

```csharp
public Task Completed { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Stopping**

```csharp
public CancellationToken Stopping { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **Stopped**

```csharp
public CancellationToken Stopped { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Constructors

### **SqlMessageReceiver(ClientContext, SqlReceiveEndpointContext)**

The basic consumer receives messages pushed from the broker.

```csharp
public SqlMessageReceiver(ClientContext client, SqlReceiveEndpointContext context)
```

#### Parameters

`client` [ClientContext](../masstransit-sqltransport/clientcontext)<br/>
The model context for the consumer

`context` [SqlReceiveEndpointContext](../masstransit-sqltransport/sqlreceiveendpointcontext)<br/>
The topology

## Methods

### **ActiveAndActualAgentsCompleted(StopContext)**

```csharp
protected Task ActiveAndActualAgentsCompleted(StopContext context)
```

#### Parameters

`context` [StopContext](../../masstransit-abstractions/masstransit/stopcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **MessageHandled()**

```csharp
public void MessageHandled()
```
