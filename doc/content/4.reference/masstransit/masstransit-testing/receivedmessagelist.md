---

title: ReceivedMessageList

---

# ReceivedMessageList

Namespace: MassTransit.Testing

```csharp
public class ReceivedMessageList : AsyncElementList<IReceivedMessage>, IAsyncElementList<IReceivedMessage>, IReceivedMessageList
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncElementList\<IReceivedMessage\>](../masstransit-testing-implementations/asyncelementlist-1) → [ReceivedMessageList](../masstransit-testing/receivedmessagelist)<br/>
Implements [IAsyncElementList\<IReceivedMessage\>](../masstransit-testing/iasyncelementlist-1), [IReceivedMessageList](../masstransit-testing/ireceivedmessagelist)

## Constructors

### **ReceivedMessageList(TimeSpan, CancellationToken)**

```csharp
public ReceivedMessageList(TimeSpan timeout, CancellationToken testCompleted)
```

#### Parameters

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`testCompleted` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **Select\<T\>(CancellationToken)**

```csharp
public IEnumerable<IReceivedMessage<T>> Select<T>(CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IEnumerable\<IReceivedMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Select\<T\>(FilterDelegate\<IReceivedMessage\<T\>\>, CancellationToken)**

```csharp
public IEnumerable<IReceivedMessage<T>> Select<T>(FilterDelegate<IReceivedMessage<T>> filter, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`filter` [FilterDelegate\<IReceivedMessage\<T\>\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IEnumerable\<IReceivedMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **SelectAsync(Action\<ReceivedMessageFilter\>, CancellationToken)**

```csharp
public IAsyncEnumerable<IReceivedMessage> SelectAsync(Action<ReceivedMessageFilter> apply, CancellationToken cancellationToken)
```

#### Parameters

`apply` [Action\<ReceivedMessageFilter\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IAsyncEnumerable\<IReceivedMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **SelectAsync\<T\>(CancellationToken)**

```csharp
public IAsyncEnumerable<IReceivedMessage<T>> SelectAsync<T>(CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IAsyncEnumerable\<IReceivedMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **SelectAsync\<T\>(FilterDelegate\<IReceivedMessage\<T\>\>, CancellationToken)**

```csharp
public IAsyncEnumerable<IReceivedMessage<T>> SelectAsync<T>(FilterDelegate<IReceivedMessage<T>> filter, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`filter` [FilterDelegate\<IReceivedMessage\<T\>\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IAsyncEnumerable\<IReceivedMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **Any(Action\<ReceivedMessageFilter\>, CancellationToken)**

```csharp
public Task<bool> Any(Action<ReceivedMessageFilter> apply, CancellationToken cancellationToken)
```

#### Parameters

`apply` [Action\<ReceivedMessageFilter\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Any\<T\>(CancellationToken)**

```csharp
public Task<bool> Any<T>(CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Any\<T\>(FilterDelegate\<IReceivedMessage\<T\>\>, CancellationToken)**

```csharp
public Task<bool> Any<T>(FilterDelegate<IReceivedMessage<T>> filter, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`filter` [FilterDelegate\<IReceivedMessage\<T\>\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Add\<T\>(ConsumeContext\<T\>)**

```csharp
public void Add<T>(ConsumeContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

### **Add\<T\>(ConsumeContext\<T\>, Exception)**

```csharp
public void Add<T>(ConsumeContext<T> context, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
