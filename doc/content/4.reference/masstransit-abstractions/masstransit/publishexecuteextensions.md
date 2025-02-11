---

title: PublishExecuteExtensions

---

# PublishExecuteExtensions

Namespace: MassTransit

```csharp
public static class PublishExecuteExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishExecuteExtensions](../masstransit/publishexecuteextensions)

## Methods

### **Publish\<T\>(IPublishEndpoint, T, Action\<PublishContext\<T\>\>, CancellationToken)**

Publish a message, using a callback to modify the publish context instead of building a pipe from scratch

```csharp
public static Task Publish<T>(IPublishEndpoint endpoint, T message, Action<PublishContext<T>> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>
The endpoint to send the message

`message` T<br/>
The message

`callback` [Action\<PublishContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
To cancel the send from happening

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Publish\<T\>(IPublishEndpoint, T, Func\<PublishContext\<T\>, Task\>, CancellationToken)**

Publish a message, using a callback to modify the publish context instead of building a pipe from scratch

```csharp
public static Task Publish<T>(IPublishEndpoint endpoint, T message, Func<PublishContext<T>, Task> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>
The endpoint to send the message

`message` T<br/>
The message

`callback` [Func\<PublishContext\<T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
To cancel the send from happening

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Publish(IPublishEndpoint, Object, Action\<PublishContext\>, CancellationToken)**

Publish a message, using a callback to modify the publish context instead of building a pipe from scratch

```csharp
public static Task Publish(IPublishEndpoint endpoint, object message, Action<PublishContext> callback, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>
The endpoint to send the message

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`callback` [Action\<PublishContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
To cancel the send from happening

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Publish(IPublishEndpoint, Object, Func\<PublishContext, Task\>, CancellationToken)**

Publish a message, using a callback to modify the publish context instead of building a pipe from scratch

```csharp
public static Task Publish(IPublishEndpoint endpoint, object message, Func<PublishContext, Task> callback, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>
The endpoint to send the message

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`callback` [Func\<PublishContext, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
To cancel the send from happening

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Publish(IPublishEndpoint, Object, Type, Action\<PublishContext\>, CancellationToken)**

Publish a message, using a callback to modify the publish context instead of building a pipe from scratch

```csharp
public static Task Publish(IPublishEndpoint endpoint, object message, Type messageType, Action<PublishContext> callback, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>
The endpoint to send the message

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The message type to send the object as

`callback` [Action\<PublishContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
To cancel the send from happening

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Publish(IPublishEndpoint, Object, Type, Func\<PublishContext, Task\>, CancellationToken)**

Publish a message, using a callback to modify the publish context instead of building a pipe from scratch

```csharp
public static Task Publish(IPublishEndpoint endpoint, object message, Type messageType, Func<PublishContext, Task> callback, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>
The endpoint to send the message

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The message type to send the object as

`callback` [Func\<PublishContext, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
To cancel the send from happening

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Publish\<T\>(IPublishEndpoint, Object, Action\<PublishContext\<T\>\>, CancellationToken)**

Publish a message, using a callback to modify the publish context instead of building a pipe from scratch

```csharp
public static Task Publish<T>(IPublishEndpoint endpoint, object values, Action<PublishContext<T>> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>
The endpoint to send the message

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values that map to the object

`callback` [Action\<PublishContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
To cancel the send from happening

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Publish\<T\>(IPublishEndpoint, Object, Func\<PublishContext\<T\>, Task\>, CancellationToken)**

Publish a message, using a callback to modify the publish context instead of building a pipe from scratch

```csharp
public static Task Publish<T>(IPublishEndpoint endpoint, object values, Func<PublishContext<T>, Task> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>
The endpoint to send the message

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values that map to the object

`callback` [Func\<PublishContext\<T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
To cancel the send from happening

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ToPipe\<T\>(Action\<PublishContext\<T\>\>)**

```csharp
public static IPipe<PublishContext<T>> ToPipe<T>(Action<PublishContext<T>> callback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`callback` [Action\<PublishContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IPipe\<PublishContext\<T\>\>](../masstransit/ipipe-1)<br/>

### **ToPipe\<T\>(Func\<PublishContext\<T\>, Task\>)**

```csharp
public static IPipe<PublishContext<T>> ToPipe<T>(Func<PublishContext<T>, Task> callback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`callback` [Func\<PublishContext\<T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IPipe\<PublishContext\<T\>\>](../masstransit/ipipe-1)<br/>

### **ToPipe(Action\<PublishContext\>)**

```csharp
public static IPipe<PublishContext> ToPipe(Action<PublishContext> callback)
```

#### Parameters

`callback` [Action\<PublishContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IPipe\<PublishContext\>](../masstransit/ipipe-1)<br/>

### **ToPipe(Func\<PublishContext, Task\>)**

```csharp
public static IPipe<PublishContext> ToPipe(Func<PublishContext, Task> callback)
```

#### Parameters

`callback` [Func\<PublishContext, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IPipe\<PublishContext\>](../masstransit/ipipe-1)<br/>
