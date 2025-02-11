---

title: PublishExtensions

---

# PublishExtensions

Namespace: MassTransit

```csharp
public static class PublishExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishExtensions](../masstransit/publishextensions)

## Methods

### **Publish\<TInstance, TMessage\>(EventActivityBinder\<TInstance\>, TMessage, Action\<PublishContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance> Publish<TInstance, TMessage>(EventActivityBinder<TInstance> source, TMessage message, Action<PublishContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

`message` TMessage<br/>

`callback` [Action\<PublishContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **PublishAsync\<TInstance, TMessage\>(EventActivityBinder\<TInstance\>, Task\<TMessage\>, Action\<PublishContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance> PublishAsync<TInstance, TMessage>(EventActivityBinder<TInstance> source, Task<TMessage> message, Action<PublishContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [Action\<PublishContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **Publish\<TInstance, TMessage\>(EventActivityBinder\<TInstance\>, EventMessageFactory\<TInstance, TMessage\>, Action\<PublishContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance> Publish<TInstance, TMessage>(EventActivityBinder<TInstance> source, EventMessageFactory<TInstance, TMessage> messageFactory, Action<PublishContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

`messageFactory` [EventMessageFactory\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/eventmessagefactory-2)<br/>

`callback` [Action\<PublishContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **PublishAsync\<TInstance, TMessage\>(EventActivityBinder\<TInstance\>, AsyncEventMessageFactory\<TInstance, TMessage\>, Action\<PublishContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance> PublishAsync<TInstance, TMessage>(EventActivityBinder<TInstance> source, AsyncEventMessageFactory<TInstance, TMessage> messageFactory, Action<PublishContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

`messageFactory` [AsyncEventMessageFactory\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-2)<br/>

`callback` [Action\<PublishContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **PublishAsync\<TInstance, TMessage\>(EventActivityBinder\<TInstance\>, Func\<BehaviorContext\<TInstance\>, Task\<SendTuple\<TMessage\>\>\>, Action\<PublishContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance> PublishAsync<TInstance, TMessage>(EventActivityBinder<TInstance> source, Func<BehaviorContext<TInstance>, Task<SendTuple<TMessage>>> messageFactory, Action<PublishContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

`messageFactory` [Func\<BehaviorContext\<TInstance\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<PublishContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **Publish\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, TMessage, Action\<PublishContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance, TData> Publish<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, TMessage message, Action<PublishContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`message` TMessage<br/>

`callback` [Action\<PublishContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **PublishAsync\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, Task\<TMessage\>, Action\<PublishContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance, TData> PublishAsync<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, Task<TMessage> message, Action<PublishContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [Action\<PublishContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Publish\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, EventMessageFactory\<TInstance, TData, TMessage\>, Action\<PublishContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance, TData> Publish<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, EventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<PublishContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`messageFactory` [EventMessageFactory\<TInstance, TData, TMessage\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>

`callback` [Action\<PublishContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **PublishAsync\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, AsyncEventMessageFactory\<TInstance, TData, TMessage\>, Action\<PublishContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance, TData> PublishAsync<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<PublishContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`messageFactory` [AsyncEventMessageFactory\<TInstance, TData, TMessage\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>

`callback` [Action\<PublishContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **PublishAsync\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, Func\<BehaviorContext\<TInstance, TData\>, Task\<SendTuple\<TMessage\>\>\>, Action\<PublishContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance, TData> PublishAsync<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, Func<BehaviorContext<TInstance, TData>, Task<SendTuple<TMessage>>> messageFactory, Action<PublishContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`messageFactory` [Func\<BehaviorContext\<TInstance, TData\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<PublishContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Publish\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, TMessage, Action\<PublishContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> Publish<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, TMessage message, Action<PublishContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`message` TMessage<br/>

`callback` [Action\<PublishContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **PublishAsync\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, Task\<TMessage\>, Action\<PublishContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> PublishAsync<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, Task<TMessage> message, Action<PublishContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [Action\<PublishContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Publish\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, EventExceptionMessageFactory\<TInstance, TException, TMessage\>, Action\<PublishContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> Publish<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, EventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory, Action<PublishContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`messageFactory` [EventExceptionMessageFactory\<TInstance, TException, TMessage\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-3)<br/>

`callback` [Action\<PublishContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **PublishAsync\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, AsyncEventExceptionMessageFactory\<TInstance, TException, TMessage\>, Action\<PublishContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> PublishAsync<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory, Action<PublishContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`messageFactory` [AsyncEventExceptionMessageFactory\<TInstance, TException, TMessage\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-3)<br/>

`callback` [Action\<PublishContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **PublishAsync\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, Func\<BehaviorExceptionContext\<TInstance, TException\>, Task\<SendTuple\<TMessage\>\>\>, Action\<PublishContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> PublishAsync<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, Func<BehaviorExceptionContext<TInstance, TException>, Task<SendTuple<TMessage>>> messageFactory, Action<PublishContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`messageFactory` [Func\<BehaviorExceptionContext\<TInstance, TException\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<PublishContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Publish\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, TMessage, Action\<PublishContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> Publish<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, TMessage message, Action<PublishContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`message` TMessage<br/>

`callback` [Action\<PublishContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **PublishAsync\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, Task\<TMessage\>, Action\<PublishContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> PublishAsync<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, Task<TMessage> message, Action<PublishContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [Action\<PublishContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Publish\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, EventExceptionMessageFactory\<TInstance, TData, TException, TMessage\>, Action\<PublishContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> Publish<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory, Action<PublishContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`messageFactory` [EventExceptionMessageFactory\<TInstance, TData, TException, TMessage\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-4)<br/>

`callback` [Action\<PublishContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **PublishAsync\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, AsyncEventExceptionMessageFactory\<TInstance, TData, TException, TMessage\>, Action\<PublishContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> PublishAsync<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory, Action<PublishContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`messageFactory` [AsyncEventExceptionMessageFactory\<TInstance, TData, TException, TMessage\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-4)<br/>

`callback` [Action\<PublishContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **PublishAsync\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, Func\<BehaviorExceptionContext\<TInstance, TData, TException\>, Task\<SendTuple\<TMessage\>\>\>, Action\<PublishContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> PublishAsync<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, Func<BehaviorExceptionContext<TInstance, TData, TException>, Task<SendTuple<TMessage>>> messageFactory, Action<PublishContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`messageFactory` [Func\<BehaviorExceptionContext\<TInstance, TData, TException\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<PublishContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>
