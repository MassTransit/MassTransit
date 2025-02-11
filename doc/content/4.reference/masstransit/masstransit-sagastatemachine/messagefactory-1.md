---

title: MessageFactory<T>

---

# MessageFactory\<T\>

Namespace: MassTransit.SagaStateMachine

```csharp
public static class MessageFactory<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageFactory\<T\>](../masstransit-sagastatemachine/messagefactory-1)

## Methods

### **Create(T)**

```csharp
public static TaskMessageFactory<T> Create(T message)
```

#### Parameters

`message` T<br/>

#### Returns

[TaskMessageFactory\<T\>](../masstransit-sagastatemachine/taskmessagefactory-1)<br/>

### **Create(T, IPipe\<SendContext\<T\>\>)**

```csharp
public static TaskMessageFactory<T> Create(T message, IPipe<SendContext<T>> pipe)
```

#### Parameters

`message` T<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[TaskMessageFactory\<T\>](../masstransit-sagastatemachine/taskmessagefactory-1)<br/>

### **Create(T, Action\<SendContext\<T\>\>)**

```csharp
public static TaskMessageFactory<T> Create(T message, Action<SendContext<T>> callback)
```

#### Parameters

`message` T<br/>

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[TaskMessageFactory\<T\>](../masstransit-sagastatemachine/taskmessagefactory-1)<br/>

### **Create(Task\<T\>)**

```csharp
public static TaskMessageFactory<T> Create(Task<T> factory)
```

#### Parameters

`factory` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

#### Returns

[TaskMessageFactory\<T\>](../masstransit-sagastatemachine/taskmessagefactory-1)<br/>

### **Create(Task\<T\>, IPipe\<SendContext\<T\>\>)**

```csharp
public static TaskMessageFactory<T> Create(Task<T> factory, IPipe<SendContext<T>> pipe)
```

#### Parameters

`factory` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[TaskMessageFactory\<T\>](../masstransit-sagastatemachine/taskmessagefactory-1)<br/>

### **Create(Task\<T\>, Action\<SendContext\<T\>\>)**

```csharp
public static TaskMessageFactory<T> Create(Task<T> factory, Action<SendContext<T>> callback)
```

#### Parameters

`factory` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[TaskMessageFactory\<T\>](../masstransit-sagastatemachine/taskmessagefactory-1)<br/>

### **Create\<TSaga, TMessage\>(T, SendContextCallback\<TSaga, TMessage, T\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(T message, SendContextCallback<TSaga, TMessage, T> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`message` T<br/>

`callback` [SendContextCallback\<TSaga, TMessage, T\>](../../masstransit-abstractions/masstransit/sendcontextcallback-3)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga, TMessage\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage\>(Task\<T\>, SendContextCallback\<TSaga, TMessage, T\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(Task<T> factory, SendContextCallback<TSaga, TMessage, T> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`factory` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [SendContextCallback\<TSaga, TMessage, T\>](../../masstransit-abstractions/masstransit/sendcontextcallback-3)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga, TMessage\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage\>(Func\<BehaviorContext\<TSaga, TMessage\>, Task\<SendTuple\<T\>\>\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(Func<BehaviorContext<TSaga, TMessage>, Task<SendTuple<T>>> factory)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`factory` [Func\<BehaviorContext\<TSaga, TMessage\>, Task\<SendTuple\<T\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga, TMessage\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage\>(Func\<BehaviorContext\<TSaga, TMessage\>, Task\<SendTuple\<T\>\>\>, Action\<SendContext\<T\>\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(Func<BehaviorContext<TSaga, TMessage>, Task<SendTuple<T>>> factory, Action<SendContext<T>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`factory` [Func\<BehaviorContext\<TSaga, TMessage\>, Task\<SendTuple\<T\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga, TMessage\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage\>(Func\<BehaviorContext\<TSaga, TMessage\>, Task\<SendTuple\<T\>\>\>, SendContextCallback\<TSaga, TMessage, T\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(Func<BehaviorContext<TSaga, TMessage>, Task<SendTuple<T>>> factory, SendContextCallback<TSaga, TMessage, T> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`factory` [Func\<BehaviorContext\<TSaga, TMessage\>, Task\<SendTuple\<T\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [SendContextCallback\<TSaga, TMessage, T\>](../../masstransit-abstractions/masstransit/sendcontextcallback-3)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga, TMessage\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage\>(AsyncEventMessageFactory\<TSaga, TMessage, T\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(AsyncEventMessageFactory<TSaga, TMessage, T> factory)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`factory` [AsyncEventMessageFactory\<TSaga, TMessage, T\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga, TMessage\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage\>(AsyncEventMessageFactory\<TSaga, TMessage, T\>, IPipe\<SendContext\<T\>\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(AsyncEventMessageFactory<TSaga, TMessage, T> factory, IPipe<SendContext<T>> pipe)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`factory` [AsyncEventMessageFactory\<TSaga, TMessage, T\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga, TMessage\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage\>(AsyncEventMessageFactory\<TSaga, TMessage, T\>, Action\<SendContext\<T\>\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(AsyncEventMessageFactory<TSaga, TMessage, T> factory, Action<SendContext<T>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`factory` [AsyncEventMessageFactory\<TSaga, TMessage, T\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga, TMessage\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage\>(AsyncEventMessageFactory\<TSaga, TMessage, T\>, SendContextCallback\<TSaga, TMessage, T\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(AsyncEventMessageFactory<TSaga, TMessage, T> factory, SendContextCallback<TSaga, TMessage, T> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`factory` [AsyncEventMessageFactory\<TSaga, TMessage, T\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>

`callback` [SendContextCallback\<TSaga, TMessage, T\>](../../masstransit-abstractions/masstransit/sendcontextcallback-3)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga, TMessage\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage\>(EventMessageFactory\<TSaga, TMessage, T\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(EventMessageFactory<TSaga, TMessage, T> factory)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`factory` [EventMessageFactory\<TSaga, TMessage, T\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga, TMessage\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage\>(EventMessageFactory\<TSaga, TMessage, T\>, IPipe\<SendContext\<T\>\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(EventMessageFactory<TSaga, TMessage, T> factory, IPipe<SendContext<T>> pipe)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`factory` [EventMessageFactory\<TSaga, TMessage, T\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga, TMessage\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage\>(EventMessageFactory\<TSaga, TMessage, T\>, Action\<SendContext\<T\>\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(EventMessageFactory<TSaga, TMessage, T> factory, Action<SendContext<T>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`factory` [EventMessageFactory\<TSaga, TMessage, T\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga, TMessage\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage\>(EventMessageFactory\<TSaga, TMessage, T\>, SendContextCallback\<TSaga, TMessage, T\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(EventMessageFactory<TSaga, TMessage, T> factory, SendContextCallback<TSaga, TMessage, T> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`factory` [EventMessageFactory\<TSaga, TMessage, T\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>

`callback` [SendContextCallback\<TSaga, TMessage, T\>](../../masstransit-abstractions/masstransit/sendcontextcallback-3)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga, TMessage\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage, TException\>(T, SendExceptionContextCallback\<TSaga, TMessage, TException, T\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(T message, SendExceptionContextCallback<TSaga, TMessage, TException, T> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

#### Parameters

`message` T<br/>

`callback` [SendExceptionContextCallback\<TSaga, TMessage, TException, T\>](../../masstransit-abstractions/masstransit/sendexceptioncontextcallback-4)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TMessage, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage, TException\>(Task\<T\>, SendExceptionContextCallback\<TSaga, TMessage, TException, T\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(Task<T> factory, SendExceptionContextCallback<TSaga, TMessage, TException, T> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

#### Parameters

`factory` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [SendExceptionContextCallback\<TSaga, TMessage, TException, T\>](../../masstransit-abstractions/masstransit/sendexceptioncontextcallback-4)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TMessage, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage, TException\>(Func\<BehaviorExceptionContext\<TSaga, TMessage, TException\>, Task\<SendTuple\<T\>\>\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(Func<BehaviorExceptionContext<TSaga, TMessage, TException>, Task<SendTuple<T>>> factory)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

#### Parameters

`factory` [Func\<BehaviorExceptionContext\<TSaga, TMessage, TException\>, Task\<SendTuple\<T\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TMessage, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage, TException\>(Func\<BehaviorExceptionContext\<TSaga, TMessage, TException\>, Task\<SendTuple\<T\>\>\>, Action\<SendContext\<T\>\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(Func<BehaviorExceptionContext<TSaga, TMessage, TException>, Task<SendTuple<T>>> factory, Action<SendContext<T>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

#### Parameters

`factory` [Func\<BehaviorExceptionContext\<TSaga, TMessage, TException\>, Task\<SendTuple\<T\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TMessage, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage, TException\>(Func\<BehaviorExceptionContext\<TSaga, TMessage, TException\>, Task\<SendTuple\<T\>\>\>, SendExceptionContextCallback\<TSaga, TMessage, TException, T\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(Func<BehaviorExceptionContext<TSaga, TMessage, TException>, Task<SendTuple<T>>> factory, SendExceptionContextCallback<TSaga, TMessage, TException, T> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

#### Parameters

`factory` [Func\<BehaviorExceptionContext\<TSaga, TMessage, TException\>, Task\<SendTuple\<T\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [SendExceptionContextCallback\<TSaga, TMessage, TException, T\>](../../masstransit-abstractions/masstransit/sendexceptioncontextcallback-4)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TMessage, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage, TException\>(AsyncEventExceptionMessageFactory\<TSaga, TMessage, TException, T\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(AsyncEventExceptionMessageFactory<TSaga, TMessage, TException, T> factory)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

#### Parameters

`factory` [AsyncEventExceptionMessageFactory\<TSaga, TMessage, TException, T\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-4)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TMessage, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage, TException\>(AsyncEventExceptionMessageFactory\<TSaga, TMessage, TException, T\>, IPipe\<SendContext\<T\>\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(AsyncEventExceptionMessageFactory<TSaga, TMessage, TException, T> factory, IPipe<SendContext<T>> pipe)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

#### Parameters

`factory` [AsyncEventExceptionMessageFactory\<TSaga, TMessage, TException, T\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-4)<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TMessage, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage, TException\>(AsyncEventExceptionMessageFactory\<TSaga, TMessage, TException, T\>, Action\<SendContext\<T\>\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(AsyncEventExceptionMessageFactory<TSaga, TMessage, TException, T> factory, Action<SendContext<T>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

#### Parameters

`factory` [AsyncEventExceptionMessageFactory\<TSaga, TMessage, TException, T\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-4)<br/>

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TMessage, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage, TException\>(AsyncEventExceptionMessageFactory\<TSaga, TMessage, TException, T\>, SendExceptionContextCallback\<TSaga, TMessage, TException, T\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(AsyncEventExceptionMessageFactory<TSaga, TMessage, TException, T> factory, SendExceptionContextCallback<TSaga, TMessage, TException, T> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

#### Parameters

`factory` [AsyncEventExceptionMessageFactory\<TSaga, TMessage, TException, T\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-4)<br/>

`callback` [SendExceptionContextCallback\<TSaga, TMessage, TException, T\>](../../masstransit-abstractions/masstransit/sendexceptioncontextcallback-4)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TMessage, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage, TException\>(EventExceptionMessageFactory\<TSaga, TMessage, TException, T\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(EventExceptionMessageFactory<TSaga, TMessage, TException, T> factory)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

#### Parameters

`factory` [EventExceptionMessageFactory\<TSaga, TMessage, TException, T\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-4)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TMessage, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage, TException\>(EventExceptionMessageFactory\<TSaga, TMessage, TException, T\>, IPipe\<SendContext\<T\>\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(EventExceptionMessageFactory<TSaga, TMessage, TException, T> factory, IPipe<SendContext<T>> pipe)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

#### Parameters

`factory` [EventExceptionMessageFactory\<TSaga, TMessage, TException, T\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-4)<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TMessage, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage, TException\>(EventExceptionMessageFactory\<TSaga, TMessage, TException, T\>, Action\<SendContext\<T\>\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(EventExceptionMessageFactory<TSaga, TMessage, TException, T> factory, Action<SendContext<T>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

#### Parameters

`factory` [EventExceptionMessageFactory\<TSaga, TMessage, TException, T\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-4)<br/>

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TMessage, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TMessage, TException\>(EventExceptionMessageFactory\<TSaga, TMessage, TException, T\>, SendExceptionContextCallback\<TSaga, TMessage, TException, T\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(EventExceptionMessageFactory<TSaga, TMessage, TException, T> factory, SendExceptionContextCallback<TSaga, TMessage, TException, T> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

#### Parameters

`factory` [EventExceptionMessageFactory\<TSaga, TMessage, TException, T\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-4)<br/>

`callback` [SendExceptionContextCallback\<TSaga, TMessage, TException, T\>](../../masstransit-abstractions/masstransit/sendexceptioncontextcallback-4)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TMessage, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga\>(T, SendContextCallback\<TSaga, T\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(T message, SendContextCallback<TSaga, T> callback)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`message` T<br/>

`callback` [SendContextCallback\<TSaga, T\>](../../masstransit-abstractions/masstransit/sendcontextcallback-2)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga\>(Task\<T\>, SendContextCallback\<TSaga, T\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(Task<T> factory, SendContextCallback<TSaga, T> callback)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`factory` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [SendContextCallback\<TSaga, T\>](../../masstransit-abstractions/masstransit/sendcontextcallback-2)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TException\>(T, SendExceptionContextCallback\<TSaga, TException, T\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(T message, SendExceptionContextCallback<TSaga, TException, T> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

#### Parameters

`message` T<br/>

`callback` [SendExceptionContextCallback\<TSaga, TException, T\>](../../masstransit-abstractions/masstransit/sendexceptioncontextcallback-3)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TException\>(Task\<T\>, SendExceptionContextCallback\<TSaga, TException, T\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(Task<T> factory, SendExceptionContextCallback<TSaga, TException, T> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

#### Parameters

`factory` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [SendExceptionContextCallback\<TSaga, TException, T\>](../../masstransit-abstractions/masstransit/sendexceptioncontextcallback-3)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga\>(Func\<BehaviorContext\<TSaga\>, Task\<SendTuple\<T\>\>\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(Func<BehaviorContext<TSaga>, Task<SendTuple<T>>> factory)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`factory` [Func\<BehaviorContext\<TSaga\>, Task\<SendTuple\<T\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga\>(Func\<BehaviorContext\<TSaga\>, Task\<SendTuple\<T\>\>\>, Action\<SendContext\<T\>\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(Func<BehaviorContext<TSaga>, Task<SendTuple<T>>> factory, Action<SendContext<T>> callback)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`factory` [Func\<BehaviorContext\<TSaga\>, Task\<SendTuple\<T\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga\>(Func\<BehaviorContext\<TSaga\>, Task\<SendTuple\<T\>\>\>, SendContextCallback\<TSaga, T\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(Func<BehaviorContext<TSaga>, Task<SendTuple<T>>> factory, SendContextCallback<TSaga, T> callback)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`factory` [Func\<BehaviorContext\<TSaga\>, Task\<SendTuple\<T\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [SendContextCallback\<TSaga, T\>](../../masstransit-abstractions/masstransit/sendcontextcallback-2)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga\>(AsyncEventMessageFactory\<TSaga, T\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(AsyncEventMessageFactory<TSaga, T> factory)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`factory` [AsyncEventMessageFactory\<TSaga, T\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-2)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga\>(AsyncEventMessageFactory\<TSaga, T\>, IPipe\<SendContext\<T\>\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(AsyncEventMessageFactory<TSaga, T> factory, IPipe<SendContext<T>> pipe)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`factory` [AsyncEventMessageFactory\<TSaga, T\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-2)<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga\>(AsyncEventMessageFactory\<TSaga, T\>, Action\<SendContext\<T\>\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(AsyncEventMessageFactory<TSaga, T> factory, Action<SendContext<T>> callback)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`factory` [AsyncEventMessageFactory\<TSaga, T\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-2)<br/>

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga\>(AsyncEventMessageFactory\<TSaga, T\>, SendContextCallback\<TSaga, T\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(AsyncEventMessageFactory<TSaga, T> factory, SendContextCallback<TSaga, T> callback)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`factory` [AsyncEventMessageFactory\<TSaga, T\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-2)<br/>

`callback` [SendContextCallback\<TSaga, T\>](../../masstransit-abstractions/masstransit/sendcontextcallback-2)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga\>(EventMessageFactory\<TSaga, T\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(EventMessageFactory<TSaga, T> factory)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`factory` [EventMessageFactory\<TSaga, T\>](../../masstransit-abstractions/masstransit/eventmessagefactory-2)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga\>(EventMessageFactory\<TSaga, T\>, IPipe\<SendContext\<T\>\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(EventMessageFactory<TSaga, T> factory, IPipe<SendContext<T>> pipe)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`factory` [EventMessageFactory\<TSaga, T\>](../../masstransit-abstractions/masstransit/eventmessagefactory-2)<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga\>(EventMessageFactory\<TSaga, T\>, Action\<SendContext\<T\>\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(EventMessageFactory<TSaga, T> factory, Action<SendContext<T>> callback)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`factory` [EventMessageFactory\<TSaga, T\>](../../masstransit-abstractions/masstransit/eventmessagefactory-2)<br/>

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga\>(EventMessageFactory\<TSaga, T\>, SendContextCallback\<TSaga, T\>)**

```csharp
public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(EventMessageFactory<TSaga, T> factory, SendContextCallback<TSaga, T> callback)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`factory` [EventMessageFactory\<TSaga, T\>](../../masstransit-abstractions/masstransit/eventmessagefactory-2)<br/>

`callback` [SendContextCallback\<TSaga, T\>](../../masstransit-abstractions/masstransit/sendcontextcallback-2)<br/>

#### Returns

[ContextMessageFactory\<BehaviorContext\<TSaga\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TException\>(Func\<BehaviorExceptionContext\<TSaga, TException\>, Task\<SendTuple\<T\>\>\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(Func<BehaviorExceptionContext<TSaga, TException>, Task<SendTuple<T>>> factory)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

#### Parameters

`factory` [Func\<BehaviorExceptionContext\<TSaga, TException\>, Task\<SendTuple\<T\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TException\>(Func\<BehaviorExceptionContext\<TSaga, TException\>, Task\<SendTuple\<T\>\>\>, Action\<SendContext\<T\>\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(Func<BehaviorExceptionContext<TSaga, TException>, Task<SendTuple<T>>> factory, Action<SendContext<T>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

#### Parameters

`factory` [Func\<BehaviorExceptionContext\<TSaga, TException\>, Task\<SendTuple\<T\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TException\>(Func\<BehaviorExceptionContext\<TSaga, TException\>, Task\<SendTuple\<T\>\>\>, SendExceptionContextCallback\<TSaga, TException, T\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(Func<BehaviorExceptionContext<TSaga, TException>, Task<SendTuple<T>>> factory, SendExceptionContextCallback<TSaga, TException, T> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

#### Parameters

`factory` [Func\<BehaviorExceptionContext\<TSaga, TException\>, Task\<SendTuple\<T\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [SendExceptionContextCallback\<TSaga, TException, T\>](../../masstransit-abstractions/masstransit/sendexceptioncontextcallback-3)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TException\>(AsyncEventExceptionMessageFactory\<TSaga, TException, T\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(AsyncEventExceptionMessageFactory<TSaga, TException, T> factory)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

#### Parameters

`factory` [AsyncEventExceptionMessageFactory\<TSaga, TException, T\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-3)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TException\>(AsyncEventExceptionMessageFactory\<TSaga, TException, T\>, IPipe\<SendContext\<T\>\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(AsyncEventExceptionMessageFactory<TSaga, TException, T> factory, IPipe<SendContext<T>> pipe)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

#### Parameters

`factory` [AsyncEventExceptionMessageFactory\<TSaga, TException, T\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-3)<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TException\>(AsyncEventExceptionMessageFactory\<TSaga, TException, T\>, Action\<SendContext\<T\>\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(AsyncEventExceptionMessageFactory<TSaga, TException, T> factory, Action<SendContext<T>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

#### Parameters

`factory` [AsyncEventExceptionMessageFactory\<TSaga, TException, T\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-3)<br/>

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TException\>(AsyncEventExceptionMessageFactory\<TSaga, TException, T\>, SendExceptionContextCallback\<TSaga, TException, T\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(AsyncEventExceptionMessageFactory<TSaga, TException, T> factory, SendExceptionContextCallback<TSaga, TException, T> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

#### Parameters

`factory` [AsyncEventExceptionMessageFactory\<TSaga, TException, T\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-3)<br/>

`callback` [SendExceptionContextCallback\<TSaga, TException, T\>](../../masstransit-abstractions/masstransit/sendexceptioncontextcallback-3)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TException\>(EventExceptionMessageFactory\<TSaga, TException, T\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(EventExceptionMessageFactory<TSaga, TException, T> factory)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

#### Parameters

`factory` [EventExceptionMessageFactory\<TSaga, TException, T\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-3)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TException\>(EventExceptionMessageFactory\<TSaga, TException, T\>, IPipe\<SendContext\<T\>\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(EventExceptionMessageFactory<TSaga, TException, T> factory, IPipe<SendContext<T>> pipe)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

#### Parameters

`factory` [EventExceptionMessageFactory\<TSaga, TException, T\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-3)<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TException\>(EventExceptionMessageFactory\<TSaga, TException, T\>, Action\<SendContext\<T\>\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(EventExceptionMessageFactory<TSaga, TException, T> factory, Action<SendContext<T>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

#### Parameters

`factory` [EventExceptionMessageFactory\<TSaga, TException, T\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-3)<br/>

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **Create\<TSaga, TException\>(EventExceptionMessageFactory\<TSaga, TException, T\>, SendExceptionContextCallback\<TSaga, TException, T\>)**

```csharp
public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(EventExceptionMessageFactory<TSaga, TException, T> factory, SendExceptionContextCallback<TSaga, TException, T> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

#### Parameters

`factory` [EventExceptionMessageFactory\<TSaga, TException, T\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-3)<br/>

`callback` [SendExceptionContextCallback\<TSaga, TException, T\>](../../masstransit-abstractions/masstransit/sendexceptioncontextcallback-3)<br/>

#### Returns

[ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TException\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>
