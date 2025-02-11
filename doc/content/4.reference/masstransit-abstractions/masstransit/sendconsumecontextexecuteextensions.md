---

title: SendConsumeContextExecuteExtensions

---

# SendConsumeContextExecuteExtensions

Namespace: MassTransit

```csharp
public static class SendConsumeContextExecuteExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendConsumeContextExecuteExtensions](../masstransit/sendconsumecontextexecuteextensions)

## Methods

### **Send\<T\>(ConsumeContext, Uri, T, Action\<SendContext\<T\>\>)**

Send a message

```csharp
public static Task Send<T>(ConsumeContext context, Uri destinationAddress, T message, Action<SendContext<T>> callback)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`destinationAddress` Uri<br/>

`message` T<br/>
The message

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the send context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send\<T\>(ConsumeContext, Uri, T, Func\<SendContext\<T\>, Task\>)**

Send a message

```csharp
public static Task Send<T>(ConsumeContext context, Uri destinationAddress, T message, Func<SendContext<T>, Task> callback)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`destinationAddress` Uri<br/>

`message` T<br/>
The message

`callback` [Func\<SendContext\<T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the send context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send(ConsumeContext, Uri, Object, Action\<SendContext\>)**

Send a message

```csharp
public static Task Send(ConsumeContext context, Uri destinationAddress, object message, Action<SendContext> callback)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`destinationAddress` Uri<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`callback` [Action\<SendContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the send context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send(ConsumeContext, Uri, Object, Func\<SendContext, Task\>)**

Send a message

```csharp
public static Task Send(ConsumeContext context, Uri destinationAddress, object message, Func<SendContext, Task> callback)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`destinationAddress` Uri<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`callback` [Func\<SendContext, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the send context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send(ConsumeContext, Uri, Object, Type, Action\<SendContext\>)**

Send a message

```csharp
public static Task Send(ConsumeContext context, Uri destinationAddress, object message, Type messageType, Action<SendContext> callback)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`destinationAddress` Uri<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`callback` [Action\<SendContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the send context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send(ConsumeContext, Uri, Object, Type, Func\<SendContext, Task\>)**

Send a message

```csharp
public static Task Send(ConsumeContext context, Uri destinationAddress, object message, Type messageType, Func<SendContext, Task> callback)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`destinationAddress` Uri<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`callback` [Func\<SendContext, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the send context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send\<T\>(ConsumeContext, Uri, Object, Action\<SendContext\<T\>\>)**

Send a message

```csharp
public static Task Send<T>(ConsumeContext context, Uri destinationAddress, object values, Action<SendContext<T>> callback)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`destinationAddress` Uri<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the send context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send\<T\>(ConsumeContext, Uri, Object, Func\<SendContext\<T\>, Task\>)**

Send a message

```csharp
public static Task Send<T>(ConsumeContext context, Uri destinationAddress, object values, Func<SendContext<T>, Task> callback)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`destinationAddress` Uri<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`callback` [Func\<SendContext\<T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the send context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker
