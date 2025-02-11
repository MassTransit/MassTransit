---

title: DynamicInitializeContext<TMessage, TInput>

---

# DynamicInitializeContext\<TMessage, TInput\>

Namespace: MassTransit.Initializers.Contexts

```csharp
public class DynamicInitializeContext<TMessage, TInput> : DynamicInitializeContext<TMessage>, InitializeContext<TMessage>, InitializeContext, PipeContext, InitializeContext<TMessage, TInput>
```

#### Type Parameters

`TMessage`<br/>

`TInput`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ProxyPipeContext](../../masstransit-abstractions/masstransit-middleware/proxypipecontext) → [DynamicInitializeContext\<TMessage\>](../masstransit-initializers-contexts/dynamicinitializecontext-1) → [DynamicInitializeContext\<TMessage, TInput\>](../masstransit-initializers-contexts/dynamicinitializecontext-2)<br/>
Implements [InitializeContext\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1), [InitializeContext](../../masstransit-abstractions/masstransit-initializers/initializecontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [InitializeContext\<TMessage, TInput\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-2)

## Properties

### **HasInput**

```csharp
public bool HasInput { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Input**

```csharp
public TInput Input { get; }
```

#### Property Value

TInput<br/>

### **Message**

```csharp
public TMessage Message { get; }
```

#### Property Value

TMessage<br/>

### **MessageType**

```csharp
public Type MessageType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **Depth**

```csharp
public int Depth { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Parent**

```csharp
public InitializeContext Parent { get; }
```

#### Property Value

[InitializeContext](../../masstransit-abstractions/masstransit-initializers/initializecontext)<br/>

### **CancellationToken**

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Constructors

### **DynamicInitializeContext(InitializeContext, TMessage, TInput)**

```csharp
public DynamicInitializeContext(InitializeContext context, TMessage message, TInput input)
```

#### Parameters

`context` [InitializeContext](../../masstransit-abstractions/masstransit-initializers/initializecontext)<br/>

`message` TMessage<br/>

`input` TInput<br/>
