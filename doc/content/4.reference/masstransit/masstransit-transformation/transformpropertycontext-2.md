---

title: TransformPropertyContext<TProperty, TMessage>

---

# TransformPropertyContext\<TProperty, TMessage\>

Namespace: MassTransit.Transformation

A transform property context, which includes the , as well as the current input property value, if present.

```csharp
public interface TransformPropertyContext<TProperty, TMessage> : TransformContext<TMessage>, TransformContext, PipeContext, MessageContext
```

#### Type Parameters

`TProperty`<br/>

`TMessage`<br/>

Implements [TransformContext\<TMessage\>](../../masstransit-abstractions/masstransit/transformcontext-1), [TransformContext](../../masstransit-abstractions/masstransit/transformcontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [MessageContext](../../masstransit-abstractions/masstransit/messagecontext)

## Properties

### **HasValue**

True if the value is present from the source

```csharp
public abstract bool HasValue { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Value**

The value

```csharp
public abstract TProperty Value { get; }
```

#### Property Value

TProperty<br/>
