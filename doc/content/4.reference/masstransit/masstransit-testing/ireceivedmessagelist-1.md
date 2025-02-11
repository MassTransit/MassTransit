---

title: IReceivedMessageList<T>

---

# IReceivedMessageList\<T\>

Namespace: MassTransit.Testing

```csharp
public interface IReceivedMessageList<T> : IAsyncElementList<IReceivedMessage<T>>
```

#### Type Parameters

`T`<br/>

Implements [IAsyncElementList\<IReceivedMessage\<T\>\>](../masstransit-testing/iasyncelementlist-1)

## Methods

### **Select(CancellationToken)**

```csharp
IEnumerable<IReceivedMessage<T>> Select(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IEnumerable\<IReceivedMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **SelectAsync(CancellationToken)**

```csharp
IAsyncEnumerable<IReceivedMessage<T>> SelectAsync(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IAsyncEnumerable\<IReceivedMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **Any(CancellationToken)**

```csharp
Task<bool> Any(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
