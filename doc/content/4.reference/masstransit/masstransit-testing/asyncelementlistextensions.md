---

title: AsyncElementListExtensions

---

# AsyncElementListExtensions

Namespace: MassTransit.Testing

```csharp
public static class AsyncElementListExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncElementListExtensions](../masstransit-testing/asyncelementlistextensions)

## Methods

### **First\<TElement\>(IAsyncEnumerable\<TElement\>)**

```csharp
public static Task<TElement> First<TElement>(IAsyncEnumerable<TElement> elements)
```

#### Type Parameters

`TElement`<br/>

#### Parameters

`elements` [IAsyncEnumerable\<TElement\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

#### Returns

[Task\<TElement\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Count\<TElement\>(IAsyncEnumerable\<TElement\>)**

```csharp
public static Task<int> Count<TElement>(IAsyncEnumerable<TElement> elements)
```

#### Type Parameters

`TElement`<br/>

#### Parameters

`elements` [IAsyncEnumerable\<TElement\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

#### Returns

[Task\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Count\<TElement\>(IAsyncElementList\<TElement\>, CancellationToken)**

```csharp
public static int Count<TElement>(IAsyncElementList<TElement> elements, CancellationToken cancellationToken)
```

#### Type Parameters

`TElement`<br/>

#### Parameters

`elements` [IAsyncElementList\<TElement\>](../masstransit-testing/iasyncelementlist-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Take\<TElement\>(IAsyncEnumerable\<TElement\>, Int32)**

```csharp
public static IAsyncEnumerable<TElement> Take<TElement>(IAsyncEnumerable<TElement> elements, int quantity)
```

#### Type Parameters

`TElement`<br/>

#### Parameters

`elements` [IAsyncEnumerable\<TElement\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

`quantity` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[IAsyncEnumerable\<TElement\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **FirstOrDefault\<TElement\>(IAsyncEnumerable\<TElement\>)**

```csharp
public static Task<TElement> FirstOrDefault<TElement>(IAsyncEnumerable<TElement> elements)
```

#### Type Parameters

`TElement`<br/>

#### Parameters

`elements` [IAsyncEnumerable\<TElement\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

#### Returns

[Task\<TElement\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Any\<TElement\>(IAsyncEnumerable\<TElement\>)**

```csharp
public static Task<bool> Any<TElement>(IAsyncEnumerable<TElement> elements)
```

#### Type Parameters

`TElement`<br/>

#### Parameters

`elements` [IAsyncEnumerable\<TElement\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Select\<TElement, TResult\>(IAsyncEnumerable\<TElement\>)**

```csharp
public static IAsyncEnumerable<TResult> Select<TElement, TResult>(IAsyncEnumerable<TElement> elements)
```

#### Type Parameters

`TElement`<br/>

`TResult`<br/>

#### Parameters

`elements` [IAsyncEnumerable\<TElement\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

#### Returns

[IAsyncEnumerable\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **Deconstruct(ISentMessage, Object, SendContext)**

```csharp
public static void Deconstruct(ISentMessage sent, out object message, out SendContext context)
```

#### Parameters

`sent` [ISentMessage](../masstransit-testing/isentmessage)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`context` [SendContext](../../masstransit-abstractions/masstransit/sendcontext)<br/>

### **Deconstruct\<TMessage\>(ISentMessage\<TMessage\>, TMessage, SendContext)**

```csharp
public static void Deconstruct<TMessage>(ISentMessage<TMessage> sent, out TMessage message, out SendContext context)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`sent` [ISentMessage\<TMessage\>](../masstransit-testing/isentmessage-1)<br/>

`message` TMessage<br/>

`context` [SendContext](../../masstransit-abstractions/masstransit/sendcontext)<br/>
