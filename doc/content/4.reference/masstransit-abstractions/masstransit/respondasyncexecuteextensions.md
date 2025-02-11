---

title: RespondAsyncExecuteExtensions

---

# RespondAsyncExecuteExtensions

Namespace: MassTransit

```csharp
public static class RespondAsyncExecuteExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RespondAsyncExecuteExtensions](../masstransit/respondasyncexecuteextensions)

## Methods

### **RespondAsync\<T\>(ConsumeContext, T, Action\<SendContext\<T\>\>)**

Responds to the current message immediately, returning the Task for the
 sending message. The caller may choose to await the response to ensure it was sent, or
 allow the framework to wait for it (which will happen automatically before the message is acked)

```csharp
public static Task RespondAsync<T>(ConsumeContext context, T message, Action<SendContext<T>> callback)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The context to send the message

`message` T<br/>
The message

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the send context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **RespondAsync\<T\>(ConsumeContext, T, Func\<SendContext\<T\>, Task\>)**

Responds to the current message immediately, returning the Task for the
 sending message. The caller may choose to await the response to ensure it was sent, or
 allow the framework to wait for it (which will happen automatically before the message is acked)

```csharp
public static Task RespondAsync<T>(ConsumeContext context, T message, Func<SendContext<T>, Task> callback)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The context to send the message

`message` T<br/>
The message

`callback` [Func\<SendContext\<T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the send context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **RespondAsync(ConsumeContext, Object, Action\<SendContext\>)**

Responds to the current message immediately, returning the Task for the
 sending message. The caller may choose to await the response to ensure it was sent, or
 allow the framework to wait for it (which will happen automatically before the message is acked)

```csharp
public static Task RespondAsync(ConsumeContext context, object message, Action<SendContext> callback)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The context to send the message

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`callback` [Action\<SendContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the send context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **RespondAsync(ConsumeContext, Object, Func\<SendContext, Task\>)**

Responds to the current message immediately, returning the Task for the
 sending message. The caller may choose to await the response to ensure it was sent, or
 allow the framework to wait for it (which will happen automatically before the message is acked)

```csharp
public static Task RespondAsync(ConsumeContext context, object message, Func<SendContext, Task> callback)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The context to send the message

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`callback` [Func\<SendContext, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the send context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **RespondAsync(ConsumeContext, Object, Type, Action\<SendContext\>)**

Responds to the current message immediately, returning the Task for the
 sending message. The caller may choose to await the response to ensure it was sent, or
 allow the framework to wait for it (which will happen automatically before the message is acked)

```csharp
public static Task RespondAsync(ConsumeContext context, object message, Type messageType, Action<SendContext> callback)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The context to send the message

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The message type to send the object as

`callback` [Action\<SendContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the send context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **RespondAsync(ConsumeContext, Object, Type, Func\<SendContext, Task\>)**

Responds to the current message immediately, returning the Task for the
 sending message. The caller may choose to await the response to ensure it was sent, or
 allow the framework to wait for it (which will happen automatically before the message is acked)

```csharp
public static Task RespondAsync(ConsumeContext context, object message, Type messageType, Func<SendContext, Task> callback)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The context to send the message

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The message type to send the object as

`callback` [Func\<SendContext, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the send context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **RespondAsync\<T\>(ConsumeContext, Object, Action\<SendContext\<T\>\>)**

Responds to the current message immediately, returning the Task for the
 sending message. The caller may choose to await the response to ensure it was sent, or
 allow the framework to wait for it (which will happen automatically before the message is acked)

```csharp
public static Task RespondAsync<T>(ConsumeContext context, object values, Action<SendContext<T>> callback)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The context to send the message

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values that map to the object

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the send context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **RespondAsync\<T\>(ConsumeContext, Object, Func\<SendContext\<T\>, Task\>)**

Responds to the current message immediately, returning the Task for the
 sending message. The caller may choose to await the response to ensure it was sent, or
 allow the framework to wait for it (which will happen automatically before the message is acked)

```csharp
public static Task RespondAsync<T>(ConsumeContext context, object values, Func<SendContext<T>, Task> callback)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The context to send the message

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values that map to the object

`callback` [Func\<SendContext\<T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the send context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker
