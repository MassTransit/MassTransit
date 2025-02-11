---

title: ScopeInitializeContext

---

# ScopeInitializeContext

Namespace: MassTransit.Initializers.Contexts

```csharp
public class ScopeInitializeContext : ScopePipeContext, InitializeContext, PipeContext
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopePipeContext](../../masstransit-abstractions/masstransit-middleware/scopepipecontext) → [ScopeInitializeContext](../masstransit-initializers-contexts/scopeinitializecontext)<br/>
Implements [InitializeContext](../../masstransit-abstractions/masstransit-initializers/initializecontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)

## Properties

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

### **ScopeInitializeContext(PipeContext)**

```csharp
public ScopeInitializeContext(PipeContext context)
```

#### Parameters

`context` [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)<br/>

## Methods

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
