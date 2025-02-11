---

title: PipeContextHandle<TContext>

---

# PipeContextHandle\<TContext\>

Namespace: MassTransit

A handle to a PipeContext instance (of type ), which can be disposed
 once it is no longer needed (or can no longer be used).

```csharp
public interface PipeContextHandle<TContext> : IAsyncDisposable
```

#### Type Parameters

`TContext`<br/>

Implements [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **IsDisposed**

True if the context has been disposed (and can no longer be used)

```csharp
public abstract bool IsDisposed { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Context**

The  context

```csharp
public abstract Task<TContext> Context { get; }
```

#### Property Value

[Task\<TContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
