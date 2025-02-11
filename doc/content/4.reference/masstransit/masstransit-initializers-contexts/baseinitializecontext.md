---

title: BaseInitializeContext

---

# BaseInitializeContext

Namespace: MassTransit.Initializers.Contexts

```csharp
public class BaseInitializeContext : BasePipeContext, PipeContext, InitializeContext
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BasePipeContext](../../masstransit-abstractions/masstransit-middleware/basepipecontext) → [BaseInitializeContext](../masstransit-initializers-contexts/baseinitializecontext)<br/>
Implements [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [InitializeContext](../../masstransit-abstractions/masstransit-initializers/initializecontext)

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

### **BaseInitializeContext(CancellationToken)**

```csharp
public BaseInitializeContext(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

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
