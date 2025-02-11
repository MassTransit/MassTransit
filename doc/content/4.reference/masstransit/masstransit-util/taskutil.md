---

title: TaskUtil

---

# TaskUtil

Namespace: MassTransit.Util

```csharp
public static class TaskUtil
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TaskUtil](../masstransit-util/taskutil)

## Properties

### **Completed**

```csharp
public static Task Completed { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **False**

```csharp
public static Task<bool> False { get; }
```

#### Property Value

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **True**

```csharp
public static Task<bool> True { get; }
```

#### Property Value

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Methods

### **Default\<T\>()**

Returns a completed task with the default value for

```csharp
public static Task<T> Default<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Faulted\<T\>(Exception)**

Returns a faulted task with the specified exception (creating using a [TaskCompletionSource\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcompletionsource-1))

```csharp
public static Task<T> Faulted<T>(Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Cancelled\<T\>()**

Returns a cancelled task for the specified type.

```csharp
public static Task<T> Cancelled<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetTask\<T\>(TaskCreationOptions)**

Creates a new [TaskCompletionSource\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcompletionsource-1), and ensures the TaskCreationOptions.RunContinuationsAsynchronously
 flag is specified (if available).

```csharp
public static TaskCompletionSource<T> GetTask<T>(TaskCreationOptions options)
```

#### Type Parameters

`T`<br/>

#### Parameters

`options` [TaskCreationOptions](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcreationoptions)<br/>

#### Returns

[TaskCompletionSource\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcompletionsource-1)<br/>

### **GetTask(TaskCreationOptions)**

Creates a new TaskCompletionSource and ensures the TaskCreationOptions.RunContinuationsAsynchronously
 flag is specified (if available).

```csharp
public static TaskCompletionSource<bool> GetTask(TaskCreationOptions options)
```

#### Parameters

`options` [TaskCreationOptions](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcreationoptions)<br/>

#### Returns

[TaskCompletionSource\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcompletionsource-1)<br/>

### **RegisterTask(CancellationToken, Task)**

Register a callback on the  which completes the resulting task.

```csharp
public static CancellationTokenRegistration RegisterTask(CancellationToken cancellationToken, out Task cancelTask)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`cancelTask` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

#### Returns

[CancellationTokenRegistration](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokenregistration)<br/>

#### Exceptions

[ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception)<br/>

### **RegisterIfCanBeCanceled(CancellationToken, CancellationTokenSource)**

```csharp
public static CancellationTokenRegistration RegisterIfCanBeCanceled(CancellationToken cancellationToken, CancellationTokenSource source)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`source` [CancellationTokenSource](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource)<br/>

#### Returns

[CancellationTokenRegistration](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokenregistration)<br/>

### **SetCompleted(TaskCompletionSource\<Boolean\>)**

Sets the source to completed using TrySetResult

```csharp
public static void SetCompleted(TaskCompletionSource<bool> source)
```

#### Parameters

`source` [TaskCompletionSource\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcompletionsource-1)<br/>

### **Await(Func\<Task\>, CancellationToken)**

```csharp
public static void Await(Func<Task> taskFactory, CancellationToken cancellationToken)
```

#### Parameters

`taskFactory` [Func\<Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **Await(Task, CancellationToken)**

```csharp
public static void Await(Task task, CancellationToken cancellationToken)
```

#### Parameters

`task` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **Await\<T\>(Func\<Task\<T\>\>, CancellationToken)**

```csharp
public static T Await<T>(Func<Task<T>> taskFactory, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`taskFactory` [Func\<Task\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

T<br/>
