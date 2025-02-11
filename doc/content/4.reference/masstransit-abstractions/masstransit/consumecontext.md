---

title: ConsumeContext

---

# ConsumeContext

Namespace: MassTransit

```csharp
public interface ConsumeContext : PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector
```

Implements [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector)

## Properties

### **ReceiveContext**

The received message context

```csharp
public abstract ReceiveContext ReceiveContext { get; }
```

#### Property Value

[ReceiveContext](../masstransit/receivecontext)<br/>

### **SerializerContext**

The serializer context from message deserialization

```csharp
public abstract SerializerContext SerializerContext { get; }
```

#### Property Value

[SerializerContext](../masstransit/serializercontext)<br/>

### **ConsumeCompleted**

An awaitable task that is completed once the consume context is completed

```csharp
public abstract Task ConsumeCompleted { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SupportedMessageTypes**

Returns the supported message types from the message

```csharp
public abstract IEnumerable<string> SupportedMessageTypes { get; }
```

#### Property Value

[IEnumerable\<String\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Methods

### **HasMessageType(Type)**

Returns true if the specified message type is contained in the serialized message

```csharp
bool HasMessageType(Type messageType)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetMessage\<T\>(ConsumeContext\<T\>)**

Returns the specified message type if available, otherwise returns false

```csharp
bool TryGetMessage<T>(out ConsumeContext<T> consumeContext)
```

#### Type Parameters

`T`<br/>

#### Parameters

`consumeContext` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **AddConsumeTask(Task)**

Add a task that must complete before the consume is completed

```csharp
void AddConsumeTask(Task task)
```

#### Parameters

`task` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RespondAsync\<T\>(T)**

Responds to the current message immediately, returning the Task for the
 sending message. The caller may choose to await the response to ensure it was sent, or
 allow the framework to wait for it (which will happen automatically before the message is acknowledged)

```csharp
Task RespondAsync<T>(T message)
```

#### Type Parameters

`T`<br/>
The type of the message to respond with.

#### Parameters

`message` T<br/>
The message to send in response

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RespondAsync\<T\>(T, IPipe\<SendContext\<T\>\>)**

Responds to the current message immediately, returning the Task for the
 sending message. The caller may choose to await the response to ensure it was sent, or
 allow the framework to wait for it (which will happen automatically before the message is acknowledged)

```csharp
Task RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
```

#### Type Parameters

`T`<br/>
The type of the message to respond with.

#### Parameters

`message` T<br/>
The message to send in response

`sendPipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>
The pipe used to customize the response send context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RespondAsync\<T\>(T, IPipe\<SendContext\>)**

Responds to the current message immediately, returning the Task for the
 sending message. The caller may choose to await the response to ensure it was sent, or
 allow the framework to wait for it (which will happen automatically before the message is acknowledged)

```csharp
Task RespondAsync<T>(T message, IPipe<SendContext> sendPipe)
```

#### Type Parameters

`T`<br/>
The type of the message to respond with.

#### Parameters

`message` T<br/>
The message to send in response

`sendPipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>
The pipe used to customize the response send context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RespondAsync(Object)**

Responds to the current message immediately, returning the Task for the
 sending message. The caller may choose to await the response to ensure it was sent, or
 allow the framework to wait for it (which will happen automatically before the message is acknowledged)

```csharp
Task RespondAsync(object message)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message to send

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RespondAsync(Object, Type)**

Responds to the current message immediately, returning the Task for the
 sending message. The caller may choose to await the response to ensure it was sent, or
 allow the framework to wait for it (which will happen automatically before the message is acknowledged)

```csharp
Task RespondAsync(object message, Type messageType)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message to send

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The message type to send

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RespondAsync(Object, IPipe\<SendContext\>)**

Responds to the current message immediately, returning the Task for the
 sending message. The caller may choose to await the response to ensure it was sent, or
 allow the framework to wait for it (which will happen automatically before the message is acknowledged)

```csharp
Task RespondAsync(object message, IPipe<SendContext> sendPipe)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message to send

`sendPipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RespondAsync(Object, Type, IPipe\<SendContext\>)**

Responds to the current message immediately, returning the Task for the
 sending message. The caller may choose to await the response to ensure it was sent, or
 allow the framework to wait for it (which will happen automatically before the message is acknowledged)

```csharp
Task RespondAsync(object message, Type messageType, IPipe<SendContext> sendPipe)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message to send

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The message type to send

`sendPipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RespondAsync\<T\>(Object)**

Responds to the current message immediately, returning the Task for the
 sending message. The caller may choose to await the response to ensure it was sent, or
 allow the framework to wait for it (which will happen automatically before the message is acknowledged)

```csharp
Task RespondAsync<T>(object values)
```

#### Type Parameters

`T`<br/>
The type of the message to respond with.

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values for the message properties

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RespondAsync\<T\>(Object, IPipe\<SendContext\<T\>\>)**

Responds to the current message immediately, returning the Task for the
 sending message. The caller may choose to await the response to ensure it was sent, or
 allow the framework to wait for it (which will happen automatically before the message is acknowledged)

```csharp
Task RespondAsync<T>(object values, IPipe<SendContext<T>> sendPipe)
```

#### Type Parameters

`T`<br/>
The type of the message to respond with.

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values for the message properties

`sendPipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RespondAsync\<T\>(Object, IPipe\<SendContext\>)**

Responds to the current message immediately, returning the Task for the
 sending message. The caller may choose to await the response to ensure it was sent, or
 allow the framework to wait for it (which will happen automatically before the message is acknowledged)

```csharp
Task RespondAsync<T>(object values, IPipe<SendContext> sendPipe)
```

#### Type Parameters

`T`<br/>
The type of the message to respond with.

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values for the message properties

`sendPipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Respond\<T\>(T)**

Adds a response to the message being consumed, which will be sent once the consumer
 has completed. The message is not acknowledged until the response is acknowledged.

```csharp
void Respond<T>(T message)
```

#### Type Parameters

`T`<br/>
The type of the message to respond with.

#### Parameters

`message` T<br/>
The message to send in response

### **NotifyConsumed\<T\>(ConsumeContext\<T\>, TimeSpan, String)**

Notify that the message has been consumed -- note that this is internal, and should not be called by a consumer.

```csharp
Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The consumer type

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **NotifyFaulted\<T\>(ConsumeContext\<T\>, TimeSpan, String, Exception)**

Notify that a message consumer has faulted -- note that this is internal, and should not be called by a consumer

```csharp
Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The message consumer type

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
The exception that occurred

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
