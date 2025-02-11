---

title: IPublishEndpoint

---

# IPublishEndpoint

Namespace: MassTransit

A publish endpoint lets the underlying transport determine the actual endpoint to which
 the message is sent. For example, an exchange on RabbitMQ and a topic on Azure Service bus.

```csharp
public interface IPublishEndpoint : IPublishObserverConnector
```

Implements [IPublishObserverConnector](../masstransit/ipublishobserverconnector)

## Methods

### **Publish\<T\>(T, CancellationToken)**

Publishes a message to all subscribed consumers for the message type as specified
 by the generic parameter. The second parameter allows the caller to customize the
 outgoing publish context and set things like headers on the message.

```csharp
Task Publish<T>(T message, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The type of the message

#### Parameters

`message` T<br/>
The messages to be published

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish\<T\>(T, IPipe\<PublishContext\<T\>\>, CancellationToken)**

Publishes a message to all subscribed consumers for the message type as specified
 by the generic parameter. The second parameter allows the caller to customize the
 outgoing publish context and set things like headers on the message.

```csharp
Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The type of the message

#### Parameters

`message` T<br/>
The messages to be published

`publishPipe` [IPipe\<PublishContext\<T\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish\<T\>(T, IPipe\<PublishContext\>, CancellationToken)**

Publishes a message to all subscribed consumers for the message type as specified
 by the generic parameter. The second parameter allows the caller to customize the
 outgoing publish context and set things like headers on the message.

```csharp
Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The type of the message

#### Parameters

`message` T<br/>
The messages to be published

`publishPipe` [IPipe\<PublishContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish(Object, CancellationToken)**

Publishes an object as a message.

```csharp
Task Publish(object message, CancellationToken cancellationToken)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish(Object, IPipe\<PublishContext\>, CancellationToken)**

Publishes an object as a message.

```csharp
Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`publishPipe` [IPipe\<PublishContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish(Object, Type, CancellationToken)**

Publishes an object as a message, using the message type specified. If the object cannot be cast
 to the specified message type, an exception will be thrown.

```csharp
Task Publish(object message, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type of the message (use message.GetType() if desired)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish(Object, Type, IPipe\<PublishContext\>, CancellationToken)**

Publishes an object as a message, using the message type specified. If the object cannot be cast
 to the specified message type, an exception will be thrown.

```csharp
Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type of the message (use message.GetType() if desired)

`publishPipe` [IPipe\<PublishContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish\<T\>(Object, CancellationToken)**

[IPublishEndpoint.Publish\<T\>(T, CancellationToken)](ipublishendpoint#publishtt-cancellationtoken): this is a "dynamically"
 typed overload - give it an interface as its type parameter,
 and a loosely typed dictionary of values and the MassTransit
 underlying infrastructure will populate an object instance
 with the passed values. It actually does this with DynamicProxy
 in the background.

```csharp
Task Publish<T>(object values, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The type of the interface or non-sealed class with all-virtual members.

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The dictionary of values to place in the object instance to implement the interface.

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish\<T\>(Object, IPipe\<PublishContext\<T\>\>, CancellationToken)**

[IPublishEndpoint.Publish\<T\>(T, CancellationToken)](ipublishendpoint#publishtt-cancellationtoken): this
 overload further takes an action; it allows you to set [PublishContext](../masstransit/publishcontext)
 meta-data. Also [IPublishEndpoint.Publish\<T\>(T, CancellationToken)](ipublishendpoint#publishtt-cancellationtoken).

```csharp
Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The type of the message to publish

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The dictionary of values to become hydrated and published under the type of the interface.

`publishPipe` [IPipe\<PublishContext\<T\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish\<T\>(Object, IPipe\<PublishContext\>, CancellationToken)**

[IPublishEndpoint.Publish\<T\>(T, CancellationToken)](ipublishendpoint#publishtt-cancellationtoken): this
 overload further takes an action; it allows you to set [PublishContext](../masstransit/publishcontext)
 meta-data. Also [IPublishEndpoint.Publish\<T\>(T, CancellationToken)](ipublishendpoint#publishtt-cancellationtoken).

```csharp
Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The type of the message to publish

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The dictionary of values to become hydrated and published under the type of the interface.

`publishPipe` [IPipe\<PublishContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
