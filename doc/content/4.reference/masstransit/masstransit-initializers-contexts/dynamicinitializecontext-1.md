---

title: DynamicInitializeContext<TMessage>

---

# DynamicInitializeContext\<TMessage\>

Namespace: MassTransit.Initializers.Contexts

```csharp
public class DynamicInitializeContext<TMessage> : ProxyPipeContext, InitializeContext<TMessage>, InitializeContext, PipeContext
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ProxyPipeContext](../../masstransit-abstractions/masstransit-middleware/proxypipecontext) → [DynamicInitializeContext\<TMessage\>](../masstransit-initializers-contexts/dynamicinitializecontext-1)<br/>
Implements [InitializeContext\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1), [InitializeContext](../../masstransit-abstractions/masstransit-initializers/initializecontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)

## Properties

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

### **DynamicInitializeContext(InitializeContext, TMessage)**

```csharp
public DynamicInitializeContext(InitializeContext context, TMessage message)
```

#### Parameters

`context` [InitializeContext](../../masstransit-abstractions/masstransit-initializers/initializecontext)<br/>

`message` TMessage<br/>

## Methods

### **CreateInputContext\<T\>(T)**

```csharp
public InitializeContext<TMessage, T> CreateInputContext<T>(T input)
```

#### Type Parameters

`T`<br/>

#### Parameters

`input` T<br/>

#### Returns

[InitializeContext\<TMessage, T\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-2)<br/>

### **TryGetParent\<T\>(InitializeContext\<T\>)**

```csharp
public bool TryGetParent<T>(out InitializeContext<T> parentContext)
```

#### Type Parameters

`T`<br/>

#### Parameters

`parentContext` [InitializeContext\<T\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **CreateMessageContext\<T\>(T)**

```csharp
public InitializeContext<T> CreateMessageContext<T>(T message)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

#### Returns

[InitializeContext\<T\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>
