---

title: TransformContext<TMessage>

---

# TransformContext\<TMessage\>

Namespace: MassTransit

A message transform for a single message type

```csharp
public interface TransformContext<TMessage> : TransformContext, PipeContext, MessageContext
```

#### Type Parameters

`TMessage`<br/>
The message type

Implements [TransformContext](../masstransit/transformcontext), [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext)

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
public abstract TMessage Input { get; }
```

#### Property Value

TMessage<br/>
