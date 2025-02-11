---

title: InitializeContext<TMessage>

---

# InitializeContext\<TMessage\>

Namespace: MassTransit.Initializers

The context of the message being initialized

```csharp
public interface InitializeContext<TMessage> : InitializeContext, PipeContext
```

#### Type Parameters

`TMessage`<br/>
The message type

Implements [InitializeContext](../masstransit-initializers/initializecontext), [PipeContext](../masstransit/pipecontext)

## Properties

### **MessageType**

```csharp
public abstract Type MessageType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **Message**

The message being initialized

```csharp
public abstract TMessage Message { get; }
```

#### Property Value

TMessage<br/>

## Methods

### **CreateInputContext\<T\>(T)**

```csharp
InitializeContext<TMessage, T> CreateInputContext<T>(T input)
```

#### Type Parameters

`T`<br/>

#### Parameters

`input` T<br/>

#### Returns

[InitializeContext\<TMessage, T\>](../masstransit-initializers/initializecontext-2)<br/>
