---

title: IMessageInitializer<TMessage>

---

# IMessageInitializer\<TMessage\>

Namespace: MassTransit.Initializers

A message initializer that doesn't use the input

```csharp
public interface IMessageInitializer<TMessage>
```

#### Type Parameters

`TMessage`<br/>
The message type

## Methods

### **Create(PipeContext)**

Create a message context, using  as a base for payloads, etc.

```csharp
InitializeContext<TMessage> Create(PipeContext context)
```

#### Parameters

`context` [PipeContext](../masstransit/pipecontext)<br/>

#### Returns

[InitializeContext\<TMessage\>](../masstransit-initializers/initializecontext-1)<br/>

### **Create(CancellationToken)**

Create a message context

```csharp
InitializeContext<TMessage> Create(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[InitializeContext\<TMessage\>](../masstransit-initializers/initializecontext-1)<br/>

### **Initialize(Object, CancellationToken)**

Initialize the message, using the input

```csharp
Task<InitializeContext<TMessage>> Initialize(object input, CancellationToken cancellationToken)
```

#### Parameters

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<InitializeContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Initialize(InitializeContext\<TMessage\>, Object)**

Initialize the message, using the input

```csharp
Task<InitializeContext<TMessage>> Initialize(InitializeContext<TMessage> context, object input)
```

#### Parameters

`context` [InitializeContext\<TMessage\>](../masstransit-initializers/initializecontext-1)<br/>
An existing initialize message context

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Task\<InitializeContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **InitializeMessage(PipeContext, Object, IPipe\<SendContext\<TMessage\>\>)**

Initialize the message using the input and send it to the endpoint.

```csharp
Task<SendTuple<TMessage>> InitializeMessage(PipeContext context, object input, IPipe<SendContext<TMessage>> pipe)
```

#### Parameters

`context` [PipeContext](../masstransit/pipecontext)<br/>
The base context

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The input object

`pipe` [IPipe\<SendContext\<TMessage\>\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task\<SendTuple\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **InitializeMessage(PipeContext, Object, Object[], IPipe\<SendContext\<TMessage\>\>)**

Initialize the message using the input and send it to the endpoint.

```csharp
Task<SendTuple<TMessage>> InitializeMessage(PipeContext context, object input, Object[] moreInputs, IPipe<SendContext<TMessage>> pipe)
```

#### Parameters

`context` [PipeContext](../masstransit/pipecontext)<br/>
The base context

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The input object

`moreInputs` [Object[]](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
Additional objects used to initialize the message

`pipe` [IPipe\<SendContext\<TMessage\>\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task\<SendTuple\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **InitializeMessage(Object, IPipe\<SendContext\<TMessage\>\>, CancellationToken)**

Initialize the message using the input and send it to the endpoint.

```csharp
Task<SendTuple<TMessage>> InitializeMessage(object input, IPipe<SendContext<TMessage>> pipe, CancellationToken cancellationToken)
```

#### Parameters

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The input object

`pipe` [IPipe\<SendContext\<TMessage\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<SendTuple\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
