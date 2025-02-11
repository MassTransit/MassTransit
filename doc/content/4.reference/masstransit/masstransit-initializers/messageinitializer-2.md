---

title: MessageInitializer<TMessage, TInput>

---

# MessageInitializer\<TMessage, TInput\>

Namespace: MassTransit.Initializers

Initializes a message using the input, which can include message properties, headers, etc.

```csharp
public class MessageInitializer<TMessage, TInput> : IMessageInitializer<TMessage>
```

#### Type Parameters

`TMessage`<br/>
The message type

`TInput`<br/>
The input type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageInitializer\<TMessage, TInput\>](../masstransit-initializers/messageinitializer-2)<br/>
Implements [IMessageInitializer\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/imessageinitializer-1)

## Constructors

### **MessageInitializer(IMessageFactory\<TMessage\>, IEnumerable\<IPropertyInitializer\<TMessage, TInput\>\>, IEnumerable\<IHeaderInitializer\<TMessage, TInput\>\>)**

```csharp
public MessageInitializer(IMessageFactory<TMessage> factory, IEnumerable<IPropertyInitializer<TMessage, TInput>> initializers, IEnumerable<IHeaderInitializer<TMessage, TInput>> headerInitializers)
```

#### Parameters

`factory` [IMessageFactory\<TMessage\>](../masstransit-initializers/imessagefactory-1)<br/>

`initializers` [IEnumerable\<IPropertyInitializer\<TMessage, TInput\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`headerInitializers` [IEnumerable\<IHeaderInitializer\<TMessage, TInput\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Methods

### **Create(PipeContext)**

```csharp
public InitializeContext<TMessage> Create(PipeContext context)
```

#### Parameters

`context` [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)<br/>

#### Returns

[InitializeContext\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

### **Create(CancellationToken)**

```csharp
public InitializeContext<TMessage> Create(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[InitializeContext\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

### **Initialize(Object, CancellationToken)**

```csharp
public Task<InitializeContext<TMessage>> Initialize(object input, CancellationToken cancellationToken)
```

#### Parameters

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<InitializeContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Initialize(InitializeContext\<TMessage\>, Object)**

```csharp
public Task<InitializeContext<TMessage>> Initialize(InitializeContext<TMessage> context, object input)
```

#### Parameters

`context` [InitializeContext\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Task\<InitializeContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **InitializeMessage(PipeContext, Object, IPipe\<SendContext\<TMessage\>\>)**

```csharp
public Task<SendTuple<TMessage>> InitializeMessage(PipeContext context, object input, IPipe<SendContext<TMessage>> pipe)
```

#### Parameters

`context` [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)<br/>

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task\<SendTuple\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **InitializeMessage(Object, IPipe\<SendContext\<TMessage\>\>, CancellationToken)**

```csharp
public Task<SendTuple<TMessage>> InitializeMessage(object input, IPipe<SendContext<TMessage>> pipe, CancellationToken cancellationToken)
```

#### Parameters

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<SendTuple\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **InitializeMessage(PipeContext, Object, Object[], IPipe\<SendContext\<TMessage\>\>)**

```csharp
public Task<SendTuple<TMessage>> InitializeMessage(PipeContext context, object input, Object[] moreInputs, IPipe<SendContext<TMessage>> pipe)
```

#### Parameters

`context` [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)<br/>

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`moreInputs` [Object[]](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task\<SendTuple\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
