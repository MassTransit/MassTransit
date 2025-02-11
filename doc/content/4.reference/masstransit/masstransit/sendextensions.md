---

title: SendExtensions

---

# SendExtensions

Namespace: MassTransit

```csharp
public static class SendExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendExtensions](../masstransit/sendextensions)

## Methods

### **Send\<TSaga, TMessage\>(EventActivityBinder\<TSaga\>, Uri, TMessage, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga> Send<TSaga, TMessage>(EventActivityBinder<TSaga> source, Uri destinationAddress, TMessage message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

`destinationAddress` Uri<br/>

`message` TMessage<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **SendAsync\<TSaga, TMessage\>(EventActivityBinder\<TSaga\>, Uri, Task\<TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga> SendAsync<TSaga, TMessage>(EventActivityBinder<TSaga> source, Uri destinationAddress, Task<TMessage> message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

`destinationAddress` Uri<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **Send\<TSaga, TMessage\>(EventActivityBinder\<TSaga\>, DestinationAddressProvider\<TSaga\>, TMessage, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga> Send<TSaga, TMessage>(EventActivityBinder<TSaga> source, DestinationAddressProvider<TSaga> destinationAddressProvider, TMessage message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

`destinationAddressProvider` [DestinationAddressProvider\<TSaga\>](../../masstransit-abstractions/masstransit/destinationaddressprovider-1)<br/>

`message` TMessage<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **SendAsync\<TSaga, TMessage\>(EventActivityBinder\<TSaga\>, DestinationAddressProvider\<TSaga\>, Task\<TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga> SendAsync<TSaga, TMessage>(EventActivityBinder<TSaga> source, DestinationAddressProvider<TSaga> destinationAddressProvider, Task<TMessage> message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

`destinationAddressProvider` [DestinationAddressProvider\<TSaga\>](../../masstransit-abstractions/masstransit/destinationaddressprovider-1)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **Send\<TSaga, TMessage\>(EventActivityBinder\<TSaga\>, Uri, EventMessageFactory\<TSaga, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga> Send<TSaga, TMessage>(EventActivityBinder<TSaga> source, Uri destinationAddress, EventMessageFactory<TSaga, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

`destinationAddress` Uri<br/>

`messageFactory` [EventMessageFactory\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/eventmessagefactory-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **SendAsync\<TSaga, TMessage\>(EventActivityBinder\<TSaga\>, Uri, AsyncEventMessageFactory\<TSaga, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga> SendAsync<TSaga, TMessage>(EventActivityBinder<TSaga> source, Uri destinationAddress, AsyncEventMessageFactory<TSaga, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

`destinationAddress` Uri<br/>

`messageFactory` [AsyncEventMessageFactory\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **SendAsync\<TSaga, TMessage\>(EventActivityBinder\<TSaga\>, Uri, Func\<BehaviorContext\<TSaga\>, Task\<SendTuple\<TMessage\>\>\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga> SendAsync<TSaga, TMessage>(EventActivityBinder<TSaga> source, Uri destinationAddress, Func<BehaviorContext<TSaga>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

`destinationAddress` Uri<br/>

`messageFactory` [Func\<BehaviorContext\<TSaga\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **Send\<TSaga, TMessage\>(EventActivityBinder\<TSaga\>, DestinationAddressProvider\<TSaga\>, EventMessageFactory\<TSaga, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga> Send<TSaga, TMessage>(EventActivityBinder<TSaga> source, DestinationAddressProvider<TSaga> destinationAddressProvider, EventMessageFactory<TSaga, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

`destinationAddressProvider` [DestinationAddressProvider\<TSaga\>](../../masstransit-abstractions/masstransit/destinationaddressprovider-1)<br/>

`messageFactory` [EventMessageFactory\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/eventmessagefactory-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **SendAsync\<TSaga, TMessage\>(EventActivityBinder\<TSaga\>, DestinationAddressProvider\<TSaga\>, AsyncEventMessageFactory\<TSaga, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga> SendAsync<TSaga, TMessage>(EventActivityBinder<TSaga> source, DestinationAddressProvider<TSaga> destinationAddressProvider, AsyncEventMessageFactory<TSaga, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

`destinationAddressProvider` [DestinationAddressProvider\<TSaga\>](../../masstransit-abstractions/masstransit/destinationaddressprovider-1)<br/>

`messageFactory` [AsyncEventMessageFactory\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **SendAsync\<TSaga, TMessage\>(EventActivityBinder\<TSaga\>, DestinationAddressProvider\<TSaga\>, Func\<BehaviorContext\<TSaga\>, Task\<SendTuple\<TMessage\>\>\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga> SendAsync<TSaga, TMessage>(EventActivityBinder<TSaga> source, DestinationAddressProvider<TSaga> destinationAddressProvider, Func<BehaviorContext<TSaga>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

`destinationAddressProvider` [DestinationAddressProvider\<TSaga\>](../../masstransit-abstractions/masstransit/destinationaddressprovider-1)<br/>

`messageFactory` [Func\<BehaviorContext\<TSaga\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **Send\<TSaga, TData, TMessage\>(EventActivityBinder\<TSaga, TData\>, Uri, TMessage, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga, TData> Send<TSaga, TData, TMessage>(EventActivityBinder<TSaga, TData> source, Uri destinationAddress, TMessage message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

`destinationAddress` Uri<br/>

`message` TMessage<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **SendAsync\<TSaga, TData, TMessage\>(EventActivityBinder\<TSaga, TData\>, Uri, Task\<TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga, TData> SendAsync<TSaga, TData, TMessage>(EventActivityBinder<TSaga, TData> source, Uri destinationAddress, Task<TMessage> message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

`destinationAddress` Uri<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Send\<TSaga, TData, TMessage\>(EventActivityBinder\<TSaga, TData\>, DestinationAddressProvider\<TSaga, TData\>, TMessage, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga, TData> Send<TSaga, TData, TMessage>(EventActivityBinder<TSaga, TData> source, DestinationAddressProvider<TSaga, TData> destinationAddressProvider, TMessage message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

`destinationAddressProvider` [DestinationAddressProvider\<TSaga, TData\>](../../masstransit-abstractions/masstransit/destinationaddressprovider-2)<br/>

`message` TMessage<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **SendAsync\<TSaga, TData, TMessage\>(EventActivityBinder\<TSaga, TData\>, DestinationAddressProvider\<TSaga, TData\>, Task\<TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga, TData> SendAsync<TSaga, TData, TMessage>(EventActivityBinder<TSaga, TData> source, DestinationAddressProvider<TSaga, TData> destinationAddressProvider, Task<TMessage> message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

`destinationAddressProvider` [DestinationAddressProvider\<TSaga, TData\>](../../masstransit-abstractions/masstransit/destinationaddressprovider-2)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Send\<TSaga, TData, TMessage\>(EventActivityBinder\<TSaga, TData\>, Uri, EventMessageFactory\<TSaga, TData, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga, TData> Send<TSaga, TData, TMessage>(EventActivityBinder<TSaga, TData> source, Uri destinationAddress, EventMessageFactory<TSaga, TData, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

`destinationAddress` Uri<br/>

`messageFactory` [EventMessageFactory\<TSaga, TData, TMessage\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **SendAsync\<TSaga, TData, TMessage\>(EventActivityBinder\<TSaga, TData\>, Uri, AsyncEventMessageFactory\<TSaga, TData, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga, TData> SendAsync<TSaga, TData, TMessage>(EventActivityBinder<TSaga, TData> source, Uri destinationAddress, AsyncEventMessageFactory<TSaga, TData, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

`destinationAddress` Uri<br/>

`messageFactory` [AsyncEventMessageFactory\<TSaga, TData, TMessage\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **SendAsync\<TSaga, TData, TMessage\>(EventActivityBinder\<TSaga, TData\>, Uri, Func\<BehaviorContext\<TSaga, TData\>, Task\<SendTuple\<TMessage\>\>\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga, TData> SendAsync<TSaga, TData, TMessage>(EventActivityBinder<TSaga, TData> source, Uri destinationAddress, Func<BehaviorContext<TSaga, TData>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

`destinationAddress` Uri<br/>

`messageFactory` [Func\<BehaviorContext\<TSaga, TData\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Send\<TSaga, TData, TMessage\>(EventActivityBinder\<TSaga, TData\>, DestinationAddressProvider\<TSaga, TData\>, EventMessageFactory\<TSaga, TData, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga, TData> Send<TSaga, TData, TMessage>(EventActivityBinder<TSaga, TData> source, DestinationAddressProvider<TSaga, TData> destinationAddressProvider, EventMessageFactory<TSaga, TData, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

`destinationAddressProvider` [DestinationAddressProvider\<TSaga, TData\>](../../masstransit-abstractions/masstransit/destinationaddressprovider-2)<br/>

`messageFactory` [EventMessageFactory\<TSaga, TData, TMessage\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **SendAsync\<TSaga, TData, TMessage\>(EventActivityBinder\<TSaga, TData\>, DestinationAddressProvider\<TSaga, TData\>, AsyncEventMessageFactory\<TSaga, TData, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga, TData> SendAsync<TSaga, TData, TMessage>(EventActivityBinder<TSaga, TData> source, DestinationAddressProvider<TSaga, TData> destinationAddressProvider, AsyncEventMessageFactory<TSaga, TData, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

`destinationAddressProvider` [DestinationAddressProvider\<TSaga, TData\>](../../masstransit-abstractions/masstransit/destinationaddressprovider-2)<br/>

`messageFactory` [AsyncEventMessageFactory\<TSaga, TData, TMessage\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **SendAsync\<TSaga, TData, TMessage\>(EventActivityBinder\<TSaga, TData\>, DestinationAddressProvider\<TSaga, TData\>, Func\<BehaviorContext\<TSaga, TData\>, Task\<SendTuple\<TMessage\>\>\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga, TData> SendAsync<TSaga, TData, TMessage>(EventActivityBinder<TSaga, TData> source, DestinationAddressProvider<TSaga, TData> destinationAddressProvider, Func<BehaviorContext<TSaga, TData>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

`destinationAddressProvider` [DestinationAddressProvider\<TSaga, TData\>](../../masstransit-abstractions/masstransit/destinationaddressprovider-2)<br/>

`messageFactory` [Func\<BehaviorContext\<TSaga, TData\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Send\<TSaga, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TException\>, Uri, TMessage, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TException> Send<TSaga, TException, TMessage>(ExceptionActivityBinder<TSaga, TException> source, Uri destinationAddress, TMessage message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`destinationAddress` Uri<br/>

`message` TMessage<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **SendAsync\<TSaga, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TException\>, Uri, Task\<TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TException> SendAsync<TSaga, TException, TMessage>(ExceptionActivityBinder<TSaga, TException> source, Uri destinationAddress, Task<TMessage> message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`destinationAddress` Uri<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Send\<TSaga, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TException\>, DestinationAddressProvider\<TSaga\>, TMessage, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TException> Send<TSaga, TException, TMessage>(ExceptionActivityBinder<TSaga, TException> source, DestinationAddressProvider<TSaga> destinationAddressProvider, TMessage message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`destinationAddressProvider` [DestinationAddressProvider\<TSaga\>](../../masstransit-abstractions/masstransit/destinationaddressprovider-1)<br/>

`message` TMessage<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **SendAsync\<TSaga, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TException\>, DestinationAddressProvider\<TSaga\>, Task\<TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TException> SendAsync<TSaga, TException, TMessage>(ExceptionActivityBinder<TSaga, TException> source, DestinationAddressProvider<TSaga> destinationAddressProvider, Task<TMessage> message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`destinationAddressProvider` [DestinationAddressProvider\<TSaga\>](../../masstransit-abstractions/masstransit/destinationaddressprovider-1)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Send\<TSaga, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TException\>, Uri, EventExceptionMessageFactory\<TSaga, TException, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TException> Send<TSaga, TException, TMessage>(ExceptionActivityBinder<TSaga, TException> source, Uri destinationAddress, EventExceptionMessageFactory<TSaga, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`destinationAddress` Uri<br/>

`messageFactory` [EventExceptionMessageFactory\<TSaga, TException, TMessage\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **SendAsync\<TSaga, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TException\>, Uri, AsyncEventExceptionMessageFactory\<TSaga, TException, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TException> SendAsync<TSaga, TException, TMessage>(ExceptionActivityBinder<TSaga, TException> source, Uri destinationAddress, AsyncEventExceptionMessageFactory<TSaga, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`destinationAddress` Uri<br/>

`messageFactory` [AsyncEventExceptionMessageFactory\<TSaga, TException, TMessage\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **SendAsync\<TSaga, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TException\>, Uri, Func\<BehaviorExceptionContext\<TSaga, TException\>, Task\<SendTuple\<TMessage\>\>\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TException> SendAsync<TSaga, TException, TMessage>(ExceptionActivityBinder<TSaga, TException> source, Uri destinationAddress, Func<BehaviorExceptionContext<TSaga, TException>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`destinationAddress` Uri<br/>

`messageFactory` [Func\<BehaviorExceptionContext\<TSaga, TException\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Send\<TSaga, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TException\>, DestinationAddressProvider\<TSaga\>, EventExceptionMessageFactory\<TSaga, TException, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TException> Send<TSaga, TException, TMessage>(ExceptionActivityBinder<TSaga, TException> source, DestinationAddressProvider<TSaga> destinationAddressProvider, EventExceptionMessageFactory<TSaga, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`destinationAddressProvider` [DestinationAddressProvider\<TSaga\>](../../masstransit-abstractions/masstransit/destinationaddressprovider-1)<br/>

`messageFactory` [EventExceptionMessageFactory\<TSaga, TException, TMessage\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **SendAsync\<TSaga, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TException\>, DestinationAddressProvider\<TSaga\>, AsyncEventExceptionMessageFactory\<TSaga, TException, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TException> SendAsync<TSaga, TException, TMessage>(ExceptionActivityBinder<TSaga, TException> source, DestinationAddressProvider<TSaga> destinationAddressProvider, AsyncEventExceptionMessageFactory<TSaga, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`destinationAddressProvider` [DestinationAddressProvider\<TSaga\>](../../masstransit-abstractions/masstransit/destinationaddressprovider-1)<br/>

`messageFactory` [AsyncEventExceptionMessageFactory\<TSaga, TException, TMessage\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **SendAsync\<TSaga, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TException\>, DestinationAddressProvider\<TSaga\>, Func\<BehaviorExceptionContext\<TSaga, TException\>, Task\<SendTuple\<TMessage\>\>\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TException> SendAsync<TSaga, TException, TMessage>(ExceptionActivityBinder<TSaga, TException> source, DestinationAddressProvider<TSaga> destinationAddressProvider, Func<BehaviorExceptionContext<TSaga, TException>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`destinationAddressProvider` [DestinationAddressProvider\<TSaga\>](../../masstransit-abstractions/masstransit/destinationaddressprovider-1)<br/>

`messageFactory` [Func\<BehaviorExceptionContext\<TSaga, TException\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Send\<TSaga, TData, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TData, TException\>, Uri, TMessage, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> Send<TSaga, TData, TException, TMessage>(ExceptionActivityBinder<TSaga, TData, TException> source, Uri destinationAddress, TMessage message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`destinationAddress` Uri<br/>

`message` TMessage<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **SendAsync\<TSaga, TData, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TData, TException\>, Uri, Task\<TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> SendAsync<TSaga, TData, TException, TMessage>(ExceptionActivityBinder<TSaga, TData, TException> source, Uri destinationAddress, Task<TMessage> message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`destinationAddress` Uri<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Send\<TSaga, TData, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TData, TException\>, DestinationAddressProvider\<TSaga, TData\>, TMessage, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> Send<TSaga, TData, TException, TMessage>(ExceptionActivityBinder<TSaga, TData, TException> source, DestinationAddressProvider<TSaga, TData> destinationAddressProvider, TMessage message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`destinationAddressProvider` [DestinationAddressProvider\<TSaga, TData\>](../../masstransit-abstractions/masstransit/destinationaddressprovider-2)<br/>

`message` TMessage<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **SendAsync\<TSaga, TData, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TData, TException\>, DestinationAddressProvider\<TSaga, TData\>, Task\<TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> SendAsync<TSaga, TData, TException, TMessage>(ExceptionActivityBinder<TSaga, TData, TException> source, DestinationAddressProvider<TSaga, TData> destinationAddressProvider, Task<TMessage> message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`destinationAddressProvider` [DestinationAddressProvider\<TSaga, TData\>](../../masstransit-abstractions/masstransit/destinationaddressprovider-2)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Send\<TSaga, TData, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TData, TException\>, Uri, EventExceptionMessageFactory\<TSaga, TData, TException, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> Send<TSaga, TData, TException, TMessage>(ExceptionActivityBinder<TSaga, TData, TException> source, Uri destinationAddress, EventExceptionMessageFactory<TSaga, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`destinationAddress` Uri<br/>

`messageFactory` [EventExceptionMessageFactory\<TSaga, TData, TException, TMessage\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-4)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Send\<TSaga, TData, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TData, TException\>, DestinationAddressProvider\<TSaga, TData\>, EventExceptionMessageFactory\<TSaga, TData, TException, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> Send<TSaga, TData, TException, TMessage>(ExceptionActivityBinder<TSaga, TData, TException> source, DestinationAddressProvider<TSaga, TData> destinationAddressProvider, EventExceptionMessageFactory<TSaga, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`destinationAddressProvider` [DestinationAddressProvider\<TSaga, TData\>](../../masstransit-abstractions/masstransit/destinationaddressprovider-2)<br/>

`messageFactory` [EventExceptionMessageFactory\<TSaga, TData, TException, TMessage\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-4)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **SendAsync\<TSaga, TData, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TData, TException\>, Uri, AsyncEventExceptionMessageFactory\<TSaga, TData, TException, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> SendAsync<TSaga, TData, TException, TMessage>(ExceptionActivityBinder<TSaga, TData, TException> source, Uri destinationAddress, AsyncEventExceptionMessageFactory<TSaga, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`destinationAddress` Uri<br/>

`messageFactory` [AsyncEventExceptionMessageFactory\<TSaga, TData, TException, TMessage\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-4)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **SendAsync\<TSaga, TData, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TData, TException\>, Uri, Func\<BehaviorExceptionContext\<TSaga, TData, TException\>, Task\<SendTuple\<TMessage\>\>\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> SendAsync<TSaga, TData, TException, TMessage>(ExceptionActivityBinder<TSaga, TData, TException> source, Uri destinationAddress, Func<BehaviorExceptionContext<TSaga, TData, TException>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`destinationAddress` Uri<br/>

`messageFactory` [Func\<BehaviorExceptionContext\<TSaga, TData, TException\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **SendAsync\<TSaga, TData, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TData, TException\>, DestinationAddressProvider\<TSaga, TData\>, AsyncEventExceptionMessageFactory\<TSaga, TData, TException, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> SendAsync<TSaga, TData, TException, TMessage>(ExceptionActivityBinder<TSaga, TData, TException> source, DestinationAddressProvider<TSaga, TData> destinationAddressProvider, AsyncEventExceptionMessageFactory<TSaga, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`destinationAddressProvider` [DestinationAddressProvider\<TSaga, TData\>](../../masstransit-abstractions/masstransit/destinationaddressprovider-2)<br/>

`messageFactory` [AsyncEventExceptionMessageFactory\<TSaga, TData, TException, TMessage\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-4)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **SendAsync\<TSaga, TData, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TData, TException\>, DestinationAddressProvider\<TSaga, TData\>, Func\<BehaviorExceptionContext\<TSaga, TData, TException\>, Task\<SendTuple\<TMessage\>\>\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> SendAsync<TSaga, TData, TException, TMessage>(ExceptionActivityBinder<TSaga, TData, TException> source, DestinationAddressProvider<TSaga, TData> destinationAddressProvider, Func<BehaviorExceptionContext<TSaga, TData, TException>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`destinationAddressProvider` [DestinationAddressProvider\<TSaga, TData\>](../../masstransit-abstractions/masstransit/destinationaddressprovider-2)<br/>

`messageFactory` [Func\<BehaviorExceptionContext\<TSaga, TData, TException\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>
