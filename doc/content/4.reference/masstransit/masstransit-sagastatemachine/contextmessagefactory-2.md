---

title: ContextMessageFactory<TContext, T>

---

# ContextMessageFactory\<TContext, T\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class ContextMessageFactory<TContext, T>
```

#### Type Parameters

`TContext`<br/>

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ContextMessageFactory\<TContext, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)

## Constructors

### **ContextMessageFactory(Func\<TContext, Task\<SendTuple\<T\>\>\>)**

```csharp
public ContextMessageFactory(Func<TContext, Task<SendTuple<T>>> messageFactory)
```

#### Parameters

`messageFactory` [Func\<TContext, Task\<SendTuple\<T\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

## Methods

### **GetMessage(TContext)**

```csharp
public Task<SendTuple<T>> GetMessage(TContext context)
```

#### Parameters

`context` TContext<br/>

#### Returns

[Task\<SendTuple\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Use(TContext, Func\<TContext, SendTuple\<T\>, Task\>)**

```csharp
public Task Use(TContext context, Func<TContext, SendTuple<T>, Task> callback)
```

#### Parameters

`context` TContext<br/>

`callback` [Func\<TContext, SendTuple\<T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Use\<TResult\>(TContext, Func\<TContext, SendTuple\<T\>, Task\<TResult\>\>)**

```csharp
public Task<TResult> Use<TResult>(TContext context, Func<TContext, SendTuple<T>, Task<TResult>> callback)
```

#### Type Parameters

`TResult`<br/>

#### Parameters

`context` TContext<br/>

`callback` [Func\<TContext, SendTuple\<T\>, Task\<TResult\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

#### Returns

[Task\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
