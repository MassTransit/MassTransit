---

title: SendByConventionExtensions

---

# SendByConventionExtensions

Namespace: MassTransit

```csharp
public static class SendByConventionExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendByConventionExtensions](../masstransit/sendbyconventionextensions)

## Methods

### **Send\<TInstance, TMessage\>(EventActivityBinder\<TInstance\>, TMessage, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance> Send<TInstance, TMessage>(EventActivityBinder<TInstance> source, TMessage message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

`message` TMessage<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **SendAsync\<TInstance, TMessage\>(EventActivityBinder\<TInstance\>, Task\<TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance> SendAsync<TInstance, TMessage>(EventActivityBinder<TInstance> source, Task<TMessage> message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **Send\<TInstance, TMessage\>(EventActivityBinder\<TInstance\>, EventMessageFactory\<TInstance, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance> Send<TInstance, TMessage>(EventActivityBinder<TInstance> source, EventMessageFactory<TInstance, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

`messageFactory` [EventMessageFactory\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/eventmessagefactory-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **SendAsync\<TInstance, TMessage\>(EventActivityBinder\<TInstance\>, AsyncEventMessageFactory\<TInstance, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance> SendAsync<TInstance, TMessage>(EventActivityBinder<TInstance> source, AsyncEventMessageFactory<TInstance, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

`messageFactory` [AsyncEventMessageFactory\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **SendAsync\<TInstance, TMessage\>(EventActivityBinder\<TInstance\>, Func\<BehaviorContext\<TInstance\>, Task\<SendTuple\<TMessage\>\>\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance> SendAsync<TInstance, TMessage>(EventActivityBinder<TInstance> source, Func<BehaviorContext<TInstance>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

`messageFactory` [Func\<BehaviorContext\<TInstance\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **Send\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, TMessage, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance, TData> Send<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, TMessage message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`message` TMessage<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **SendAsync\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, Task\<TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance, TData> SendAsync<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, Task<TMessage> message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Send\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, EventMessageFactory\<TInstance, TData, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance, TData> Send<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, EventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`messageFactory` [EventMessageFactory\<TInstance, TData, TMessage\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **SendAsync\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, AsyncEventMessageFactory\<TInstance, TData, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance, TData> SendAsync<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`messageFactory` [AsyncEventMessageFactory\<TInstance, TData, TMessage\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **SendAsync\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, Func\<BehaviorContext\<TInstance, TData\>, Task\<SendTuple\<TMessage\>\>\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance, TData> SendAsync<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, Func<BehaviorContext<TInstance, TData>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`messageFactory` [Func\<BehaviorContext\<TInstance, TData\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Send\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, TMessage, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> Send<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, TMessage message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`message` TMessage<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **SendAsync\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, Task\<TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> SendAsync<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, Task<TMessage> message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Send\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, EventExceptionMessageFactory\<TInstance, TException, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> Send<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, EventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`messageFactory` [EventExceptionMessageFactory\<TInstance, TException, TMessage\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **SendAsync\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, AsyncEventExceptionMessageFactory\<TInstance, TException, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> SendAsync<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`messageFactory` [AsyncEventExceptionMessageFactory\<TInstance, TException, TMessage\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **SendAsync\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, Func\<BehaviorExceptionContext\<TInstance, TException\>, Task\<SendTuple\<TMessage\>\>\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> SendAsync<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, Func<BehaviorExceptionContext<TInstance, TException>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`messageFactory` [Func\<BehaviorExceptionContext\<TInstance, TException\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Send\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, TMessage, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> Send<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, TMessage message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`message` TMessage<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **SendAsync\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, Task\<TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> SendAsync<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, Task<TMessage> message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Send\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, EventExceptionMessageFactory\<TInstance, TData, TException, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> Send<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`messageFactory` [EventExceptionMessageFactory\<TInstance, TData, TException, TMessage\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-4)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **SendAsync\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, AsyncEventExceptionMessageFactory\<TInstance, TData, TException, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> SendAsync<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`messageFactory` [AsyncEventExceptionMessageFactory\<TInstance, TData, TException, TMessage\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-4)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **SendAsync\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, Func\<BehaviorExceptionContext\<TInstance, TData, TException\>, Task\<SendTuple\<TMessage\>\>\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> SendAsync<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, Func<BehaviorExceptionContext<TInstance, TData, TException>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`messageFactory` [Func\<BehaviorExceptionContext\<TInstance, TData, TException\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Send\<TInstance, TMessage\>(EventActivityBinder\<TInstance\>, TMessage, SendContextCallback\<TInstance, TMessage\>)**

```csharp
public static EventActivityBinder<TInstance> Send<TInstance, TMessage>(EventActivityBinder<TInstance> source, TMessage message, SendContextCallback<TInstance, TMessage> callback)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

`message` TMessage<br/>

`callback` [SendContextCallback\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/sendcontextcallback-2)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **SendAsync\<TInstance, TMessage\>(EventActivityBinder\<TInstance\>, Task\<TMessage\>, SendContextCallback\<TInstance, TMessage\>)**

```csharp
public static EventActivityBinder<TInstance> SendAsync<TInstance, TMessage>(EventActivityBinder<TInstance> source, Task<TMessage> message, SendContextCallback<TInstance, TMessage> callback)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [SendContextCallback\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/sendcontextcallback-2)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **Send\<TInstance, TMessage\>(EventActivityBinder\<TInstance\>, EventMessageFactory\<TInstance, TMessage\>, SendContextCallback\<TInstance, TMessage\>)**

```csharp
public static EventActivityBinder<TInstance> Send<TInstance, TMessage>(EventActivityBinder<TInstance> source, EventMessageFactory<TInstance, TMessage> messageFactory, SendContextCallback<TInstance, TMessage> callback)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

`messageFactory` [EventMessageFactory\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/eventmessagefactory-2)<br/>

`callback` [SendContextCallback\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/sendcontextcallback-2)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **SendAsync\<TInstance, TMessage\>(EventActivityBinder\<TInstance\>, AsyncEventMessageFactory\<TInstance, TMessage\>, SendContextCallback\<TInstance, TMessage\>)**

```csharp
public static EventActivityBinder<TInstance> SendAsync<TInstance, TMessage>(EventActivityBinder<TInstance> source, AsyncEventMessageFactory<TInstance, TMessage> messageFactory, SendContextCallback<TInstance, TMessage> callback)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

`messageFactory` [AsyncEventMessageFactory\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-2)<br/>

`callback` [SendContextCallback\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/sendcontextcallback-2)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **SendAsync\<TInstance, TMessage\>(EventActivityBinder\<TInstance\>, Func\<BehaviorContext\<TInstance\>, Task\<SendTuple\<TMessage\>\>\>, SendContextCallback\<TInstance, TMessage\>)**

```csharp
public static EventActivityBinder<TInstance> SendAsync<TInstance, TMessage>(EventActivityBinder<TInstance> source, Func<BehaviorContext<TInstance>, Task<SendTuple<TMessage>>> messageFactory, SendContextCallback<TInstance, TMessage> callback)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

`messageFactory` [Func\<BehaviorContext\<TInstance\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [SendContextCallback\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/sendcontextcallback-2)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **Send\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, TMessage, SendContextCallback\<TInstance, TData, TMessage\>)**

```csharp
public static EventActivityBinder<TInstance, TData> Send<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, TMessage message, SendContextCallback<TInstance, TData, TMessage> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`message` TMessage<br/>

`callback` [SendContextCallback\<TInstance, TData, TMessage\>](../../masstransit-abstractions/masstransit/sendcontextcallback-3)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **SendAsync\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, Task\<TMessage\>, SendContextCallback\<TInstance, TData, TMessage\>)**

```csharp
public static EventActivityBinder<TInstance, TData> SendAsync<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, Task<TMessage> message, SendContextCallback<TInstance, TData, TMessage> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [SendContextCallback\<TInstance, TData, TMessage\>](../../masstransit-abstractions/masstransit/sendcontextcallback-3)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Send\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, EventMessageFactory\<TInstance, TData, TMessage\>, SendContextCallback\<TInstance, TData, TMessage\>)**

```csharp
public static EventActivityBinder<TInstance, TData> Send<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, EventMessageFactory<TInstance, TData, TMessage> messageFactory, SendContextCallback<TInstance, TData, TMessage> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`messageFactory` [EventMessageFactory\<TInstance, TData, TMessage\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>

`callback` [SendContextCallback\<TInstance, TData, TMessage\>](../../masstransit-abstractions/masstransit/sendcontextcallback-3)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **SendAsync\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, AsyncEventMessageFactory\<TInstance, TData, TMessage\>, SendContextCallback\<TInstance, TData, TMessage\>)**

```csharp
public static EventActivityBinder<TInstance, TData> SendAsync<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory, SendContextCallback<TInstance, TData, TMessage> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`messageFactory` [AsyncEventMessageFactory\<TInstance, TData, TMessage\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>

`callback` [SendContextCallback\<TInstance, TData, TMessage\>](../../masstransit-abstractions/masstransit/sendcontextcallback-3)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **SendAsync\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, Func\<BehaviorContext\<TInstance, TData\>, Task\<SendTuple\<TMessage\>\>\>, SendContextCallback\<TInstance, TData, TMessage\>)**

```csharp
public static EventActivityBinder<TInstance, TData> SendAsync<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, Func<BehaviorContext<TInstance, TData>, Task<SendTuple<TMessage>>> messageFactory, SendContextCallback<TInstance, TData, TMessage> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`messageFactory` [Func\<BehaviorContext\<TInstance, TData\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [SendContextCallback\<TInstance, TData, TMessage\>](../../masstransit-abstractions/masstransit/sendcontextcallback-3)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Send\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, TMessage, SendExceptionContextCallback\<TInstance, TException, TMessage\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> Send<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, TMessage message, SendExceptionContextCallback<TInstance, TException, TMessage> callback)
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`message` TMessage<br/>

`callback` [SendExceptionContextCallback\<TInstance, TException, TMessage\>](../../masstransit-abstractions/masstransit/sendexceptioncontextcallback-3)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **SendAsync\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, Task\<TMessage\>, SendExceptionContextCallback\<TInstance, TException, TMessage\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> SendAsync<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, Task<TMessage> message, SendExceptionContextCallback<TInstance, TException, TMessage> callback)
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [SendExceptionContextCallback\<TInstance, TException, TMessage\>](../../masstransit-abstractions/masstransit/sendexceptioncontextcallback-3)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Send\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, EventExceptionMessageFactory\<TInstance, TException, TMessage\>, SendExceptionContextCallback\<TInstance, TException, TMessage\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> Send<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, EventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory, SendExceptionContextCallback<TInstance, TException, TMessage> callback)
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`messageFactory` [EventExceptionMessageFactory\<TInstance, TException, TMessage\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-3)<br/>

`callback` [SendExceptionContextCallback\<TInstance, TException, TMessage\>](../../masstransit-abstractions/masstransit/sendexceptioncontextcallback-3)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **SendAsync\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, AsyncEventExceptionMessageFactory\<TInstance, TException, TMessage\>, SendExceptionContextCallback\<TInstance, TException, TMessage\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> SendAsync<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory, SendExceptionContextCallback<TInstance, TException, TMessage> callback)
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`messageFactory` [AsyncEventExceptionMessageFactory\<TInstance, TException, TMessage\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-3)<br/>

`callback` [SendExceptionContextCallback\<TInstance, TException, TMessage\>](../../masstransit-abstractions/masstransit/sendexceptioncontextcallback-3)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **SendAsync\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, Func\<BehaviorExceptionContext\<TInstance, TException\>, Task\<SendTuple\<TMessage\>\>\>, SendExceptionContextCallback\<TInstance, TException, TMessage\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> SendAsync<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, Func<BehaviorExceptionContext<TInstance, TException>, Task<SendTuple<TMessage>>> messageFactory, SendExceptionContextCallback<TInstance, TException, TMessage> callback)
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`messageFactory` [Func\<BehaviorExceptionContext\<TInstance, TException\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [SendExceptionContextCallback\<TInstance, TException, TMessage\>](../../masstransit-abstractions/masstransit/sendexceptioncontextcallback-3)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Send\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, TMessage, SendExceptionContextCallback\<TInstance, TData, TException, TMessage\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> Send<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, TMessage message, SendExceptionContextCallback<TInstance, TData, TException, TMessage> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`message` TMessage<br/>

`callback` [SendExceptionContextCallback\<TInstance, TData, TException, TMessage\>](../../masstransit-abstractions/masstransit/sendexceptioncontextcallback-4)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **SendAsync\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, Task\<TMessage\>, SendExceptionContextCallback\<TInstance, TData, TException, TMessage\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> SendAsync<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, Task<TMessage> message, SendExceptionContextCallback<TInstance, TData, TException, TMessage> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [SendExceptionContextCallback\<TInstance, TData, TException, TMessage\>](../../masstransit-abstractions/masstransit/sendexceptioncontextcallback-4)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Send\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, EventExceptionMessageFactory\<TInstance, TData, TException, TMessage\>, SendExceptionContextCallback\<TInstance, TData, TException, TMessage\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> Send<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory, SendExceptionContextCallback<TInstance, TData, TException, TMessage> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`messageFactory` [EventExceptionMessageFactory\<TInstance, TData, TException, TMessage\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-4)<br/>

`callback` [SendExceptionContextCallback\<TInstance, TData, TException, TMessage\>](../../masstransit-abstractions/masstransit/sendexceptioncontextcallback-4)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **SendAsync\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, AsyncEventExceptionMessageFactory\<TInstance, TData, TException, TMessage\>, SendExceptionContextCallback\<TInstance, TData, TException, TMessage\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> SendAsync<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory, SendExceptionContextCallback<TInstance, TData, TException, TMessage> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`messageFactory` [AsyncEventExceptionMessageFactory\<TInstance, TData, TException, TMessage\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-4)<br/>

`callback` [SendExceptionContextCallback\<TInstance, TData, TException, TMessage\>](../../masstransit-abstractions/masstransit/sendexceptioncontextcallback-4)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **SendAsync\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, Func\<BehaviorExceptionContext\<TInstance, TData, TException\>, Task\<SendTuple\<TMessage\>\>\>, SendExceptionContextCallback\<TInstance, TData, TException, TMessage\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> SendAsync<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, Func<BehaviorExceptionContext<TInstance, TData, TException>, Task<SendTuple<TMessage>>> messageFactory, SendExceptionContextCallback<TInstance, TData, TException, TMessage> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`messageFactory` [Func\<BehaviorExceptionContext\<TInstance, TData, TException\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [SendExceptionContextCallback\<TInstance, TData, TException, TMessage\>](../../masstransit-abstractions/masstransit/sendexceptioncontextcallback-4)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>
