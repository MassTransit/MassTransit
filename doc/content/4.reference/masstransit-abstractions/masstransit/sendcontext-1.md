---

title: SendContext<T>

---

# SendContext\<T\>

Namespace: MassTransit

The SendContext is used to tweak the send to the endpoint

```csharp
public interface SendContext<T> : SendContext, PipeContext
```

#### Type Parameters

`T`<br/>
The message type being sent

Implements [SendContext](../masstransit/sendcontext), [PipeContext](../masstransit/pipecontext)

## Properties

### **Message**

The message being sent

```csharp
public abstract T Message { get; }
```

#### Property Value

T<br/>
