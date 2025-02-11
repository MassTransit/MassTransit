---

title: ISentMessageList

---

# ISentMessageList

Namespace: MassTransit.Testing

```csharp
public interface ISentMessageList : IAsyncElementList<ISentMessage>
```

Implements [IAsyncElementList\<ISentMessage\>](../masstransit-testing/iasyncelementlist-1)

## Methods

### **Select\<T\>(CancellationToken)**

```csharp
IEnumerable<ISentMessage<T>> Select<T>(CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IEnumerable\<ISentMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Select\<T\>(FilterDelegate\<ISentMessage\<T\>\>, CancellationToken)**

```csharp
IEnumerable<ISentMessage<T>> Select<T>(FilterDelegate<ISentMessage<T>> filter, CancellationToken cancellationToken)
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
IAsyncEnumerable<ISentMessage> SelectAsync(Action<SentMessageFilter> apply, CancellationToken cancellationToken)
```

#### Parameters

`apply` [Action\<SentMessageFilter\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IAsyncEnumerable\<ISentMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **SelectAsync\<T\>(CancellationToken)**

```csharp
IAsyncEnumerable<ISentMessage<T>> SelectAsync<T>(CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IAsyncEnumerable\<ISentMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **SelectAsync\<T\>(FilterDelegate\<ISentMessage\<T\>\>, CancellationToken)**

```csharp
IAsyncEnumerable<ISentMessage<T>> SelectAsync<T>(FilterDelegate<ISentMessage<T>> filter, CancellationToken cancellationToken)
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
Task<bool> Any(Action<SentMessageFilter> apply, CancellationToken cancellationToken)
```

#### Parameters

`apply` [Action\<SentMessageFilter\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Any\<T\>(CancellationToken)**

```csharp
Task<bool> Any<T>(CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Any\<T\>(FilterDelegate\<ISentMessage\<T\>\>, CancellationToken)**

```csharp
Task<bool> Any<T>(FilterDelegate<ISentMessage<T>> filter, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`filter` [FilterDelegate\<ISentMessage\<T\>\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
