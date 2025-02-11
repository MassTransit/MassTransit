---

title: InitializeContext<TMessage, TInput>

---

# InitializeContext\<TMessage, TInput\>

Namespace: MassTransit.Initializers

Message initialization context, which includes the message being initialized and the input
 being used to initialize the message properties.

```csharp
public interface InitializeContext<TMessage, TInput> : InitializeContext<TMessage>, InitializeContext, PipeContext
```

#### Type Parameters

`TMessage`<br/>
The message type

`TInput`<br/>
The input type

Implements [InitializeContext\<TMessage\>](../masstransit-initializers/initializecontext-1), [InitializeContext](../masstransit-initializers/initializecontext), [PipeContext](../masstransit/pipecontext)

## Properties

### **HasInput**

If true, the input is present, otherwise it equals default.

```csharp
public abstract bool HasInput { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Input**

```csharp
public abstract TInput Input { get; }
```

#### Property Value

TInput<br/>
