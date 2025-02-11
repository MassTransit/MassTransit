---

title: MessageBatch<TMessage>

---

# MessageBatch\<TMessage\>

Namespace: MassTransit.Batching

```csharp
public class MessageBatch<TMessage> : Batch<TMessage>, IEnumerable<ConsumeContext<TMessage>>, IEnumerable
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageBatch\<TMessage\>](../masstransit-batching/messagebatch-1)<br/>
Implements [Batch\<TMessage\>](../../masstransit-abstractions/masstransit/batch-1), [IEnumerable\<ConsumeContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **Mode**

```csharp
public BatchCompletionMode Mode { get; set; }
```

#### Property Value

[BatchCompletionMode](../../masstransit-abstractions/masstransit/batchcompletionmode)<br/>

### **FirstMessageReceived**

```csharp
public DateTime FirstMessageReceived { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **LastMessageReceived**

```csharp
public DateTime LastMessageReceived { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Item**

```csharp
public ConsumeContext<TMessage> Item { get; }
```

#### Property Value

[ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

### **Length**

```csharp
public int Length { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **MessageBatch(DateTime, DateTime, BatchCompletionMode, IReadOnlyList\<ConsumeContext\<TMessage\>\>)**

```csharp
public MessageBatch(DateTime firstMessageReceived, DateTime lastMessageReceived, BatchCompletionMode mode, IReadOnlyList<ConsumeContext<TMessage>> messages)
```

#### Parameters

`firstMessageReceived` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`lastMessageReceived` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`mode` [BatchCompletionMode](../../masstransit-abstractions/masstransit/batchcompletionmode)<br/>

`messages` [IReadOnlyList\<ConsumeContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)<br/>

## Methods

### **GetEnumerator()**

```csharp
public IEnumerator<ConsumeContext<TMessage>> GetEnumerator()
```

#### Returns

[IEnumerator\<ConsumeContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1)<br/>
