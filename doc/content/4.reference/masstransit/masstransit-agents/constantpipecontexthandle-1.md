---

title: ConstantPipeContextHandle<TContext>

---

# ConstantPipeContextHandle\<TContext\>

Namespace: MassTransit.Agents

```csharp
public class ConstantPipeContextHandle<TContext> : PipeContextHandle<TContext>, IAsyncDisposable
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConstantPipeContextHandle\<TContext\>](../masstransit-agents/constantpipecontexthandle-1)<br/>
Implements [PipeContextHandle\<TContext\>](../masstransit/pipecontexthandle-1), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **Context**

```csharp
public Task<TContext> Context { get; }
```

#### Property Value

[Task\<TContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Constructors

### **ConstantPipeContextHandle(TContext)**

```csharp
public ConstantPipeContextHandle(TContext context)
```

#### Parameters

`context` TContext<br/>
