---

title: ReceiveContext

---

# ReceiveContext

Namespace: MassTransit

The receive context is sent from the transport when a message is ready to be processed
 from the transport.

```csharp
public interface ReceiveContext : PipeContext
```

Implements [PipeContext](../masstransit/pipecontext)

## Properties

### **ElapsedTime**

```csharp
public abstract TimeSpan ElapsedTime { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **InputAddress**

The address on which the message was received

```csharp
public abstract Uri InputAddress { get; }
```

#### Property Value

Uri<br/>

### **ContentType**

The content type of the message, as determined by the available headers

```csharp
public abstract ContentType ContentType { get; }
```

#### Property Value

ContentType<br/>

### **Redelivered**

If True, the message is being redelivered by the transport

```csharp
public abstract bool Redelivered { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TransportHeaders**

Headers specific to the transport

```csharp
public abstract Headers TransportHeaders { get; }
```

#### Property Value

[Headers](../masstransit/headers)<br/>

### **ReceiveCompleted**

The task that is completed once all pending tasks are completed

```csharp
public abstract Task ReceiveCompleted { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **IsDelivered**

Returns true if the message was successfully consumed by at least one consumer

```csharp
public abstract bool IsDelivered { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsFaulted**

Returns true if a fault occurred during the message delivery

```csharp
public abstract bool IsFaulted { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **SendEndpointProvider**

The send endpoint provider from the transport

```csharp
public abstract ISendEndpointProvider SendEndpointProvider { get; }
```

#### Property Value

[ISendEndpointProvider](../masstransit/isendendpointprovider)<br/>

### **PublishEndpointProvider**

The publish endpoint provider from the transport

```csharp
public abstract IPublishEndpointProvider PublishEndpointProvider { get; }
```

#### Property Value

[IPublishEndpointProvider](../masstransit/ipublishendpointprovider)<br/>

### **PublishFaults**

If true (the default), faults should be published when no ResponseAddress or FaultAddress are present.

```csharp
public abstract bool PublishFaults { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Body**

The message body

```csharp
public abstract MessageBody Body { get; }
```

#### Property Value

[MessageBody](../masstransit/messagebody)<br/>

## Methods

### **NotifyConsumed\<T\>(ConsumeContext\<T\>, TimeSpan, String)**

Notify that a message has been consumed from the received context

```csharp
Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>
The consume context of the message

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time spent by the consumer

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The consumer type

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **NotifyFaulted\<T\>(ConsumeContext\<T\>, TimeSpan, String, Exception)**

Notify that a message consumer faulted

```csharp
Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>
The consume context of the message

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time spent by the consumer

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The message consumer type that faulted

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
The exception that occurred

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **NotifyFaulted(Exception)**

Notify that a message receive faulted outside of the message consumer

```csharp
Task NotifyFaulted(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
The exception that occurred

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **AddReceiveTask(Task)**

Adds a pending Task to the completion of the message receiver

```csharp
void AddReceiveTask(Task task)
```

#### Parameters

`task` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
