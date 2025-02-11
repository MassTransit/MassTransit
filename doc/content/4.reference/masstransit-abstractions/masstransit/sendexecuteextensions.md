---

title: SendExecuteExtensions

---

# SendExecuteExtensions

Namespace: MassTransit

```csharp
public static class SendExecuteExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendExecuteExtensions](../masstransit/sendexecuteextensions)

## Methods

### **Send\<T\>(ISendEndpoint, T, Action\<SendContext\<T\>\>, CancellationToken)**

Send a message, using a callback to modify the send context instead of building a pipe from scratch

```csharp
public static Task Send<T>(ISendEndpoint endpoint, T message, Action<SendContext<T>> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>
The endpoint to send the message

`message` T<br/>
The message

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
To cancel the send from happening

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send\<T\>(ISendEndpoint, T, Func\<SendContext\<T\>, Task\>, CancellationToken)**

Send a message, using a callback to modify the send context instead of building a pipe from scratch

```csharp
public static Task Send<T>(ISendEndpoint endpoint, T message, Func<SendContext<T>, Task> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>
The endpoint to send the message

`message` T<br/>
The message

`callback` [Func\<SendContext\<T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
To cancel the send from happening

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send(ISendEndpoint, Object, Action\<SendContext\>, CancellationToken)**

Send a message, using a callback to modify the send context instead of building a pipe from scratch

```csharp
public static Task Send(ISendEndpoint endpoint, object message, Action<SendContext> callback, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>
The endpoint to send the message

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`callback` [Action\<SendContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
To cancel the send from happening

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send(ISendEndpoint, Object, Func\<SendContext, Task\>, CancellationToken)**

Send a message, using a callback to modify the send context instead of building a pipe from scratch

```csharp
public static Task Send(ISendEndpoint endpoint, object message, Func<SendContext, Task> callback, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>
The endpoint to send the message

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`callback` [Func\<SendContext, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
To cancel the send from happening

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send(ISendEndpoint, Object, Type, Action\<SendContext\>, CancellationToken)**

Send a message, using a callback to modify the send context instead of building a pipe from scratch

```csharp
public static Task Send(ISendEndpoint endpoint, object message, Type messageType, Action<SendContext> callback, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>
The endpoint to send the message

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The message type to send the object as

`callback` [Action\<SendContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
To cancel the send from happening

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send(ISendEndpoint, Object, Type, Func\<SendContext, Task\>, CancellationToken)**

Send a message, using a callback to modify the send context instead of building a pipe from scratch

```csharp
public static Task Send(ISendEndpoint endpoint, object message, Type messageType, Func<SendContext, Task> callback, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>
The endpoint to send the message

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The message type to send the object as

`callback` [Func\<SendContext, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
To cancel the send from happening

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send\<T\>(ISendEndpoint, Object, Action\<SendContext\<T\>\>, CancellationToken)**

Send a message, using a callback to modify the send context instead of building a pipe from scratch

```csharp
public static Task Send<T>(ISendEndpoint endpoint, object values, Action<SendContext<T>> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>
The endpoint to send the message

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values that map to the object

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
To cancel the send from happening

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send\<T\>(ISendEndpoint, Object, Func\<SendContext\<T\>, Task\>, CancellationToken)**

Send a message, using a callback to modify the send context instead of building a pipe from scratch

```csharp
public static Task Send<T>(ISendEndpoint endpoint, object values, Func<SendContext<T>, Task> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>
The endpoint to send the message

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values that map to the object

`callback` [Func\<SendContext\<T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
To cancel the send from happening

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ToPipe\<T\>(Action\<SendContext\<T\>\>)**

```csharp
public static IPipe<SendContext<T>> ToPipe<T>(Action<SendContext<T>> callback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

### **ToPipe\<T\>(Func\<SendContext\<T\>, Task\>)**

```csharp
public static IPipe<SendContext<T>> ToPipe<T>(Func<SendContext<T>, Task> callback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`callback` [Func\<SendContext\<T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

### **ToPipe(Action\<SendContext\>)**

```csharp
public static IPipe<SendContext> ToPipe(Action<SendContext> callback)
```

#### Parameters

`callback` [Action\<SendContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

### **ToPipe(Func\<SendContext, Task\>)**

```csharp
public static IPipe<SendContext> ToPipe(Func<SendContext, Task> callback)
```

#### Parameters

`callback` [Func\<SendContext, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>
