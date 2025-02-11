---

title: SentMessageList

---

# SentMessageList

Namespace: MassTransit.Testing

```csharp
public class SentMessageList : AsyncElementList<ISentMessage>, IAsyncElementList<ISentMessage>, ISentMessageList
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncElementList\<ISentMessage\>](../masstransit-testing-implementations/asyncelementlist-1) → [SentMessageList](../masstransit-testing/sentmessagelist)<br/>
Implements [IAsyncElementList\<ISentMessage\>](../masstransit-testing/iasyncelementlist-1), [ISentMessageList](../masstransit-testing/isentmessagelist)

## Constructors

### **SentMessageList(TimeSpan, CancellationToken)**

```csharp
public SentMessageList(TimeSpan timeout, CancellationToken testCompleted)
```

#### Parameters

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`testCompleted` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **Select\<T\>(CancellationToken)**

```csharp
public IEnumerable<ISentMessage<T>> Select<T>(CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IEnumerable\<ISentMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Select\<T\>(FilterDelegate\<ISentMessage\<T\>\>, CancellationToken)**

```csharp
public IEnumerable<ISentMessage<T>> Select<T>(FilterDelegate<ISentMessage<T>> filter, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`filter` [FilterDelegate\<ISentMessage\<T\>\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IEnumerable\<ISentMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **SelectAsync(Action\<SentMessageFilter\>, CancellationToken)**

```csharp
public IAsyncEnumerable<ISentMessage> SelectAsync(Action<SentMessageFilter> apply, CancellationToken cancellationToken)
```

#### Parameters

`apply` [Action\<SentMessageFilter\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IAsyncEnumerable\<ISentMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **SelectAsync\<T\>(CancellationToken)**

```csharp
public IAsyncEnumerable<ISentMessage<T>> SelectAsync<T>(CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IAsyncEnumerable\<ISentMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **SelectAsync\<T\>(FilterDelegate\<ISentMessage\<T\>\>, CancellationToken)**

```csharp
public IAsyncEnumerable<ISentMessage<T>> SelectAsync<T>(FilterDelegate<ISentMessage<T>> filter, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`filter` [FilterDelegate\<ISentMessage\<T\>\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IAsyncEnumerable\<ISentMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **Any(Action\<SentMessageFilter\>, CancellationToken)**

```csharp
public Task<bool> Any(Action<SentMessageFilter> apply, CancellationToken cancellationToken)
```

#### Parameters

`apply` [Action\<SentMessageFilter\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

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

### **Any\<T\>(FilterDelegate\<ISentMessage\<T\>\>, CancellationToken)**

```csharp
public Task<bool> Any<T>(FilterDelegate<ISentMessage<T>> filter, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`filter` [FilterDelegate\<ISentMessage\<T\>\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Add\<T\>(SendContext\<T\>)**

```csharp
public void Add<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

### **Add\<T\>(SendContext\<T\>, Exception)**

```csharp
public void Add<T>(SendContext<T> context, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
