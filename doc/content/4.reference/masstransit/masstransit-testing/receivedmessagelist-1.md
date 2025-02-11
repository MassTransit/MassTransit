---

title: ReceivedMessageList<T>

---

# ReceivedMessageList\<T\>

Namespace: MassTransit.Testing

```csharp
public class ReceivedMessageList<T> : AsyncElementList<IReceivedMessage<T>>, IAsyncElementList<IReceivedMessage<T>>, IReceivedMessageList<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncElementList\<IReceivedMessage\<T\>\>](../masstransit-testing-implementations/asyncelementlist-1) → [ReceivedMessageList\<T\>](../masstransit-testing/receivedmessagelist-1)<br/>
Implements [IAsyncElementList\<IReceivedMessage\<T\>\>](../masstransit-testing/iasyncelementlist-1), [IReceivedMessageList\<T\>](../masstransit-testing/ireceivedmessagelist-1)

## Constructors

### **ReceivedMessageList(TimeSpan, CancellationToken)**

```csharp
public ReceivedMessageList(TimeSpan timeout, CancellationToken testCompleted)
```

#### Parameters

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`testCompleted` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **Select(CancellationToken)**

```csharp
public IEnumerable<IReceivedMessage<T>> Select(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IEnumerable\<IReceivedMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **SelectAsync(CancellationToken)**

```csharp
public IAsyncEnumerable<IReceivedMessage<T>> SelectAsync(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IAsyncEnumerable\<IReceivedMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **Any(CancellationToken)**

```csharp
public Task<bool> Any(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Add(ConsumeContext\<T\>)**

```csharp
public void Add(ConsumeContext<T> context)
```

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

### **Add(ConsumeContext\<T\>, Exception)**

```csharp
public void Add(ConsumeContext<T> context, Exception exception)
```

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
