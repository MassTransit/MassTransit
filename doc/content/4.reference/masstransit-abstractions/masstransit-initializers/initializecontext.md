---

title: InitializeContext

---

# InitializeContext

Namespace: MassTransit.Initializers

```csharp
public interface InitializeContext : PipeContext
```

Implements [PipeContext](../masstransit/pipecontext)

## Properties

### **Depth**

how deep this context is within the object graph

```csharp
public abstract int Depth { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Parent**

the parent initialize context, which is valid if the type is being initialized
 within another type

```csharp
public abstract InitializeContext Parent { get; }
```

#### Property Value

[InitializeContext](../masstransit-initializers/initializecontext)<br/>

## Methods

### **TryGetParent\<T\>(InitializeContext\<T\>)**

Return the closest parent context for the specified type, if present

```csharp
bool TryGetParent<T>(out InitializeContext<T> parentContext)
```

#### Type Parameters

`T`<br/>

#### Parameters

`parentContext` [InitializeContext\<T\>](../masstransit-initializers/initializecontext-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **CreateMessageContext\<T\>(T)**

```csharp
InitializeContext<T> CreateMessageContext<T>(T message)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

#### Returns

[InitializeContext\<T\>](../masstransit-initializers/initializecontext-1)<br/>
