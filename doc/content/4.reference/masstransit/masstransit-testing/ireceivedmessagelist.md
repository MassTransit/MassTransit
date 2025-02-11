---

title: IReceivedMessageList

---

# IReceivedMessageList

Namespace: MassTransit.Testing

```csharp
public interface IReceivedMessageList : IAsyncElementList<IReceivedMessage>
```

Implements [IAsyncElementList\<IReceivedMessage\>](../masstransit-testing/iasyncelementlist-1)

## Methods

### **Select\<T\>(CancellationToken)**

```csharp
IEnumerable<IReceivedMessage<T>> Select<T>(CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IEnumerable\<IReceivedMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Select\<T\>(FilterDelegate\<IReceivedMessage\<T\>\>, CancellationToken)**

```csharp
IEnumerable<IReceivedMessage<T>> Select<T>(FilterDelegate<IReceivedMessage<T>> filter, CancellationToken cancellationToken)
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
IAsyncEnumerable<IReceivedMessage> SelectAsync(Action<ReceivedMessageFilter> apply, CancellationToken cancellationToken)
```

#### Parameters

`apply` [Action\<ReceivedMessageFilter\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IAsyncEnumerable\<IReceivedMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **SelectAsync\<T\>(CancellationToken)**

```csharp
IAsyncEnumerable<IReceivedMessage<T>> SelectAsync<T>(CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IAsyncEnumerable\<IReceivedMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **SelectAsync\<T\>(FilterDelegate\<IReceivedMessage\<T\>\>, CancellationToken)**

```csharp
IAsyncEnumerable<IReceivedMessage<T>> SelectAsync<T>(FilterDelegate<IReceivedMessage<T>> filter, CancellationToken cancellationToken)
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
Task<bool> Any(Action<ReceivedMessageFilter> apply, CancellationToken cancellationToken)
```

#### Parameters

`apply` [Action\<ReceivedMessageFilter\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

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

### **Any\<T\>(FilterDelegate\<IReceivedMessage\<T\>\>, CancellationToken)**

```csharp
Task<bool> Any<T>(FilterDelegate<IReceivedMessage<T>> filter, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`filter` [FilterDelegate\<IReceivedMessage\<T\>\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
