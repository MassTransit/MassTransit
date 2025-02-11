---

title: PublishedMessageList

---

# PublishedMessageList

Namespace: MassTransit.Testing

```csharp
public class PublishedMessageList : AsyncElementList<IPublishedMessage>, IAsyncElementList<IPublishedMessage>, IPublishedMessageList
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncElementList\<IPublishedMessage\>](../masstransit-testing-implementations/asyncelementlist-1) → [PublishedMessageList](../masstransit-testing/publishedmessagelist)<br/>
Implements [IAsyncElementList\<IPublishedMessage\>](../masstransit-testing/iasyncelementlist-1), [IPublishedMessageList](../masstransit-testing/ipublishedmessagelist)

## Constructors

### **PublishedMessageList(TimeSpan, CancellationToken)**

```csharp
public PublishedMessageList(TimeSpan timeout, CancellationToken testCompleted)
```

#### Parameters

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`testCompleted` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **Select\<T\>(CancellationToken)**

```csharp
public IEnumerable<IPublishedMessage<T>> Select<T>(CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IEnumerable\<IPublishedMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Select\<T\>(FilterDelegate\<IPublishedMessage\<T\>\>, CancellationToken)**

```csharp
public IEnumerable<IPublishedMessage<T>> Select<T>(FilterDelegate<IPublishedMessage<T>> filter, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`filter` [FilterDelegate\<IPublishedMessage\<T\>\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IEnumerable\<IPublishedMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **SelectAsync(Action\<PublishedMessageFilter\>, CancellationToken)**

```csharp
public IAsyncEnumerable<IPublishedMessage> SelectAsync(Action<PublishedMessageFilter> apply, CancellationToken cancellationToken)
```

#### Parameters

`apply` [Action\<PublishedMessageFilter\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IAsyncEnumerable\<IPublishedMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **SelectAsync\<T\>(CancellationToken)**

```csharp
public IAsyncEnumerable<IPublishedMessage<T>> SelectAsync<T>(CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IAsyncEnumerable\<IPublishedMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **SelectAsync\<T\>(FilterDelegate\<IPublishedMessage\<T\>\>, CancellationToken)**

```csharp
public IAsyncEnumerable<IPublishedMessage<T>> SelectAsync<T>(FilterDelegate<IPublishedMessage<T>> filter, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`filter` [FilterDelegate\<IPublishedMessage\<T\>\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IAsyncEnumerable\<IPublishedMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **Any(Action\<PublishedMessageFilter\>, CancellationToken)**

```csharp
public Task<bool> Any(Action<PublishedMessageFilter> apply, CancellationToken cancellationToken)
```

#### Parameters

`apply` [Action\<PublishedMessageFilter\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

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

### **Any\<T\>(FilterDelegate\<IPublishedMessage\<T\>\>, CancellationToken)**

```csharp
public Task<bool> Any<T>(FilterDelegate<IPublishedMessage<T>> filter, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`filter` [FilterDelegate\<IPublishedMessage\<T\>\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Add\<T\>(PublishContext\<T\>)**

```csharp
public void Add<T>(PublishContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [PublishContext\<T\>](../../masstransit-abstractions/masstransit/publishcontext-1)<br/>

### **Add\<T\>(PublishContext\<T\>, Exception)**

```csharp
public void Add<T>(PublishContext<T> context, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [PublishContext\<T\>](../../masstransit-abstractions/masstransit/publishcontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
