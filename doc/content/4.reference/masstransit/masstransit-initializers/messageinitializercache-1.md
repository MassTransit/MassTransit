---

title: MessageInitializerCache<TMessage>

---

# MessageInitializerCache\<TMessage\>

Namespace: MassTransit.Initializers

```csharp
public class MessageInitializerCache<TMessage> : IMessageInitializerCache<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageInitializerCache\<TMessage\>](../masstransit-initializers/messageinitializercache-1)<br/>
Implements [IMessageInitializerCache\<TMessage\>](../masstransit-initializers/imessageinitializercache-1)

## Methods

### **GetInitializer(Type)**

Returns the initializer for the message/input type combination

```csharp
public static IMessageInitializer<TMessage> GetInitializer(Type inputType)
```

#### Parameters

`inputType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IMessageInitializer\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/imessageinitializer-1)<br/>

### **Initialize(Object, CancellationToken)**

```csharp
public static Task<InitializeContext<TMessage>> Initialize(object values, CancellationToken cancellationToken)
```

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<InitializeContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **InitializeMessage(PipeContext, Object)**

```csharp
public static Task<SendTuple<TMessage>> InitializeMessage(PipeContext context, object values)
```

#### Parameters

`context` [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Task\<SendTuple\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **InitializeMessage(PipeContext, Object, Object[], IPipe\<SendContext\<TMessage\>\>)**

```csharp
public static Task<SendTuple<TMessage>> InitializeMessage(PipeContext context, object values, Object[] moreValues, IPipe<SendContext<TMessage>> pipe)
```

#### Parameters

`context` [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`moreValues` [Object[]](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task\<SendTuple\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **InitializeMessage(PipeContext, Object, IPipe\<SendContext\<TMessage\>\>)**

```csharp
public static Task<SendTuple<TMessage>> InitializeMessage(PipeContext context, object values, IPipe<SendContext<TMessage>> pipe)
```

#### Parameters

`context` [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task\<SendTuple\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **InitializeMessage(Object, CancellationToken)**

```csharp
public static Task<SendTuple<TMessage>> InitializeMessage(object values, CancellationToken cancellationToken)
```

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<SendTuple\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **InitializeMessage(Object, IPipe\<SendContext\<TMessage\>\>, CancellationToken)**

```csharp
public static Task<SendTuple<TMessage>> InitializeMessage(object values, IPipe<SendContext<TMessage>> pipe, CancellationToken cancellationToken)
```

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<SendTuple\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Initialize(InitializeContext\<TMessage\>, Object)**

```csharp
public static Task<InitializeContext<TMessage>> Initialize(InitializeContext<TMessage> context, object values)
```

#### Parameters

`context` [InitializeContext\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Task\<InitializeContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
