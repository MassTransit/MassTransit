---

title: BatchProduceExtensions

---

# BatchProduceExtensions

Namespace: MassTransit

```csharp
public static class BatchProduceExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BatchProduceExtensions](../masstransit/batchproduceextensions)

## Methods

### **SendBatch\<T\>(ISendEndpoint, IEnumerable\<T\>, CancellationToken)**

Send a message batch

```csharp
public static Task SendBatch<T>(ISendEndpoint endpoint, IEnumerable<T> messages, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **SendBatch\<T\>(ISendEndpoint, IEnumerable\<T\>, IPipe\<SendContext\<T\>\>, CancellationToken)**

Send a message

```csharp
public static Task SendBatch<T>(ISendEndpoint endpoint, IEnumerable<T> messages, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **SendBatch\<T\>(ISendEndpoint, IEnumerable\<T\>, Action\<SendContext\<T\>\>, CancellationToken)**

Send a message batch

```csharp
public static Task SendBatch<T>(ISendEndpoint endpoint, IEnumerable<T> messages, Action<SendContext<T>> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **SendBatch\<T\>(ISendEndpoint, IEnumerable\<T\>, Func\<SendContext\<T\>, Task\>, CancellationToken)**

Send a message batch

```csharp
public static Task SendBatch<T>(ISendEndpoint endpoint, IEnumerable<T> messages, Func<SendContext<T>, Task> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`callback` [Func\<SendContext\<T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **SendBatch(ISendEndpoint, IEnumerable\<Object\>, CancellationToken)**

Send a message batch

```csharp
public static Task SendBatch(ISendEndpoint endpoint, IEnumerable<object> messages, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **SendBatch(ISendEndpoint, IEnumerable\<Object\>, IPipe\<SendContext\>, CancellationToken)**

Send a message batch

```csharp
public static Task SendBatch(ISendEndpoint endpoint, IEnumerable<object> messages, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **SendBatch(ISendEndpoint, IEnumerable\<Object\>, Action\<SendContext\>, CancellationToken)**

Send a message batch

```csharp
public static Task SendBatch(ISendEndpoint endpoint, IEnumerable<object> messages, Action<SendContext> callback, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`callback` [Action\<SendContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **SendBatch(ISendEndpoint, IEnumerable\<Object\>, Func\<SendContext, Task\>, CancellationToken)**

Send a message batch

```csharp
public static Task SendBatch(ISendEndpoint endpoint, IEnumerable<object> messages, Func<SendContext, Task> callback, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`callback` [Func\<SendContext, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **SendBatch(ISendEndpoint, IEnumerable\<Object\>, Type, CancellationToken)**

Send a message

```csharp
public static Task SendBatch(ISendEndpoint endpoint, IEnumerable<object> messages, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **SendBatch(ISendEndpoint, IEnumerable\<Object\>, Type, IPipe\<SendContext\>, CancellationToken)**

Send a message

```csharp
public static Task SendBatch(ISendEndpoint endpoint, IEnumerable<object> messages, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **SendBatch(ISendEndpoint, IEnumerable\<Object\>, Type, Action\<SendContext\>, CancellationToken)**

Send a message

```csharp
public static Task SendBatch(ISendEndpoint endpoint, IEnumerable<object> messages, Type messageType, Action<SendContext> callback, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`callback` [Action\<SendContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **SendBatch(ISendEndpoint, IEnumerable\<Object\>, Type, Func\<SendContext, Task\>, CancellationToken)**

Send a message

```csharp
public static Task SendBatch(ISendEndpoint endpoint, IEnumerable<object> messages, Type messageType, Func<SendContext, Task> callback, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`callback` [Func\<SendContext, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **PublishBatch\<T\>(IPublishEndpoint, IEnumerable\<T\>, CancellationToken)**

Send a message

```csharp
public static Task PublishBatch<T>(IPublishEndpoint endpoint, IEnumerable<T> messages, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **PublishBatch\<T\>(IPublishEndpoint, IEnumerable\<T\>, IPipe\<PublishContext\<T\>\>, CancellationToken)**

Publish a message batch

```csharp
public static Task PublishBatch<T>(IPublishEndpoint endpoint, IEnumerable<T> messages, IPipe<PublishContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`pipe` [IPipe\<PublishContext\<T\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Publish is acknowledged by the broker

### **PublishBatch\<T\>(IPublishEndpoint, IEnumerable\<T\>, Action\<PublishContext\<T\>\>, CancellationToken)**

Publish a message batch

```csharp
public static Task PublishBatch<T>(IPublishEndpoint endpoint, IEnumerable<T> messages, Action<PublishContext<T>> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`callback` [Action\<PublishContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the publish context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Publish is acknowledged by the broker

### **PublishBatch\<T\>(IPublishEndpoint, IEnumerable\<T\>, Func\<PublishContext\<T\>, Task\>, CancellationToken)**

Publish a message batch

```csharp
public static Task PublishBatch<T>(IPublishEndpoint endpoint, IEnumerable<T> messages, Func<PublishContext<T>, Task> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`callback` [Func\<PublishContext\<T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the publish context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Publish is acknowledged by the broker

### **PublishBatch(IPublishEndpoint, IEnumerable\<Object\>, CancellationToken)**

Publish a message batch

```csharp
public static Task PublishBatch(IPublishEndpoint endpoint, IEnumerable<object> messages, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Publish is acknowledged by the broker

### **PublishBatch(IPublishEndpoint, IEnumerable\<Object\>, IPipe\<PublishContext\>, CancellationToken)**

Publish a message batch

```csharp
public static Task PublishBatch(IPublishEndpoint endpoint, IEnumerable<object> messages, IPipe<PublishContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`pipe` [IPipe\<PublishContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Publish is acknowledged by the broker

### **PublishBatch(IPublishEndpoint, IEnumerable\<Object\>, Action\<PublishContext\>, CancellationToken)**

Publish a message batch

```csharp
public static Task PublishBatch(IPublishEndpoint endpoint, IEnumerable<object> messages, Action<PublishContext> callback, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`callback` [Action\<PublishContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the publish context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Publish is acknowledged by the broker

### **PublishBatch(IPublishEndpoint, IEnumerable\<Object\>, Func\<PublishContext, Task\>, CancellationToken)**

Publish a message batch

```csharp
public static Task PublishBatch(IPublishEndpoint endpoint, IEnumerable<object> messages, Func<PublishContext, Task> callback, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`callback` [Func\<PublishContext, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the publish context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Publish is acknowledged by the broker

### **PublishBatch(IPublishEndpoint, IEnumerable\<Object\>, Type, CancellationToken)**

Publish a message batch

```csharp
public static Task PublishBatch(IPublishEndpoint endpoint, IEnumerable<object> messages, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Publish is acknowledged by the broker

### **PublishBatch(IPublishEndpoint, IEnumerable\<Object\>, Type, IPipe\<PublishContext\>, CancellationToken)**

Publish a message batch

```csharp
public static Task PublishBatch(IPublishEndpoint endpoint, IEnumerable<object> messages, Type messageType, IPipe<PublishContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`pipe` [IPipe\<PublishContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Publish is acknowledged by the broker

### **PublishBatch(IPublishEndpoint, IEnumerable\<Object\>, Type, Action\<PublishContext\>, CancellationToken)**

Publish a message batch

```csharp
public static Task PublishBatch(IPublishEndpoint endpoint, IEnumerable<object> messages, Type messageType, Action<PublishContext> callback, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`callback` [Action\<PublishContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the publish context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Publish is acknowledged by the broker

### **PublishBatch(IPublishEndpoint, IEnumerable\<Object\>, Type, Func\<PublishContext, Task\>, CancellationToken)**

Publish a message batch

```csharp
public static Task PublishBatch(IPublishEndpoint endpoint, IEnumerable<object> messages, Type messageType, Func<PublishContext, Task> callback, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>
The destination endpoint

`messages` [IEnumerable\<Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`callback` [Func\<PublishContext, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the publish context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Publish is acknowledged by the broker
