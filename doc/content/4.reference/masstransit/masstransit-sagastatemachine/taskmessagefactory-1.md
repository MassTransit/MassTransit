---

title: TaskMessageFactory<T>

---

# TaskMessageFactory\<T\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class TaskMessageFactory<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TaskMessageFactory\<T\>](../masstransit-sagastatemachine/taskmessagefactory-1)

## Constructors

### **TaskMessageFactory(Task\<SendTuple\<T\>\>)**

```csharp
public TaskMessageFactory(Task<SendTuple<T>> messageFactory)
```

#### Parameters

`messageFactory` [Task\<SendTuple\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Methods

### **GetMessage()**

```csharp
public Task<SendTuple<T>> GetMessage()
```

#### Returns

[Task\<SendTuple\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Use(Func\<SendTuple\<T\>, Task\>)**

```csharp
public Task Use(Func<SendTuple<T>, Task> callback)
```

#### Parameters

`callback` [Func\<SendTuple\<T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
