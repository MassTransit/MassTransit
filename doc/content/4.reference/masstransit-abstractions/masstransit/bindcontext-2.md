---

title: BindContext<TLeft, TRight>

---

# BindContext\<TLeft, TRight\>

Namespace: MassTransit

The binding of a value to the context, which is a fancy form of Tuple

```csharp
public interface BindContext<TLeft, TRight> : PipeContext
```

#### Type Parameters

`TLeft`<br/>
The pipe context type

`TRight`<br/>
The source context type

Implements [PipeContext](../masstransit/pipecontext)

## Properties

### **Left**

```csharp
public abstract TLeft Left { get; }
```

#### Property Value

TLeft<br/>

### **Right**

```csharp
public abstract TRight Right { get; }
```

#### Property Value

TRight<br/>
