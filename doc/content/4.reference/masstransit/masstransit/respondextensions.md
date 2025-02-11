---

title: RespondExtensions

---

# RespondExtensions

Namespace: MassTransit

```csharp
public static class RespondExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RespondExtensions](../masstransit/respondextensions)

## Methods

### **Respond\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, TMessage, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance, TData> Respond<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, TMessage message, Action<SendContext<TMessage>> callback)
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

### **RespondAsync\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, Task\<TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance, TData> RespondAsync<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, Task<TMessage> message, Action<SendContext<TMessage>> callback)
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

### **Respond\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, EventMessageFactory\<TInstance, TData, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance, TData> Respond<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, EventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
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

### **RespondAsync\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, AsyncEventMessageFactory\<TInstance, TData, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance, TData> RespondAsync<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
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

### **RespondAsync\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, Func\<BehaviorContext\<TInstance, TData\>, Task\<SendTuple\<TMessage\>\>\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance, TData> RespondAsync<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, Func<BehaviorContext<TInstance, TData>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback)
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

### **Respond\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, TMessage, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> Respond<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, TMessage message, Action<SendContext<TMessage>> callback)
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

### **RespondAsync\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, Task\<TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> RespondAsync<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, Task<TMessage> message, Action<SendContext<TMessage>> callback)
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

### **Respond\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, EventExceptionMessageFactory\<TInstance, TException, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> Respond<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, EventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
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

### **RespondAsync\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, AsyncEventExceptionMessageFactory\<TInstance, TException, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> RespondAsync<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
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

### **Respond\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, TMessage, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> Respond<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, TMessage message, Action<SendContext<TMessage>> callback)
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

### **RespondAsync\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, Task\<TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> RespondAsync<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, Task<TMessage> message, Action<SendContext<TMessage>> callback)
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

### **Respond\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, EventExceptionMessageFactory\<TInstance, TData, TException, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> Respond<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
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

### **RespondAsync\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, AsyncEventExceptionMessageFactory\<TInstance, TData, TException, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> RespondAsync<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
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
