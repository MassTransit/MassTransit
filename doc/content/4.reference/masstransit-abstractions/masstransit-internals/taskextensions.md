---

title: TaskExtensions

---

# TaskExtensions

Namespace: MassTransit.Internals

```csharp
public static class TaskExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TaskExtensions](../masstransit-internals/taskextensions)

## Methods

### **OrCanceled(Task, CancellationToken)**

```csharp
public static Task OrCanceled(Task task, CancellationToken cancellationToken)
```

#### Parameters

`task` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **OrCanceled\<T\>(Task\<T\>, CancellationToken)**

```csharp
public static Task<T> OrCanceled<T>(Task<T> task, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`task` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **OrTimeout(Task, Int32, Int32, Int32, Int32, Int32, CancellationToken, String, String, Nullable\<Int32\>)**

```csharp
public static Task OrTimeout(Task task, int ms, int s, int m, int h, int d, CancellationToken cancellationToken, string memberName, string filePath, Nullable<int> lineNumber)
```

#### Parameters

`task` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

`ms` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`s` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`m` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`h` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`d` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`memberName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`filePath` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`lineNumber` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **OrTimeout(Task, TimeSpan, CancellationToken, String, String, Nullable\<Int32\>)**

```csharp
public static Task OrTimeout(Task task, TimeSpan timeout, CancellationToken cancellationToken, string memberName, string filePath, Nullable<int> lineNumber)
```

#### Parameters

`task` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`memberName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`filePath` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`lineNumber` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **OrTimeout\<T\>(Task\<T\>, Int32, Int32, Int32, Int32, Int32, CancellationToken, String, String, Nullable\<Int32\>)**

```csharp
public static Task<T> OrTimeout<T>(Task<T> task, int ms, int s, int m, int h, int d, CancellationToken cancellationToken, string memberName, string filePath, Nullable<int> lineNumber)
```

#### Type Parameters

`T`<br/>

#### Parameters

`task` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`ms` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`s` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`m` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`h` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`d` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`memberName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`filePath` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`lineNumber` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **OrTimeout\<T\>(Task\<T\>, TimeSpan, CancellationToken, String, String, Nullable\<Int32\>)**

```csharp
public static Task<T> OrTimeout<T>(Task<T> task, TimeSpan timeout, CancellationToken cancellationToken, string memberName, string filePath, Nullable<int> lineNumber)
```

#### Type Parameters

`T`<br/>

#### Parameters

`task` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`memberName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`filePath` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`lineNumber` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **IsCompletedSuccessfully(Task)**

Returns true if a Task was ran to completion (without being cancelled or faulted)

```csharp
public static bool IsCompletedSuccessfully(Task task)
```

#### Parameters

`task` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IgnoreUnobservedExceptions(Task)**

```csharp
public static void IgnoreUnobservedExceptions(Task task)
```

#### Parameters

`task` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **TrySetFromTask\<T\>(TaskCompletionSource\<T\>, Task, T)**

```csharp
public static void TrySetFromTask<T>(TaskCompletionSource<T> source, Task task, T value)
```

#### Type Parameters

`T`<br/>

#### Parameters

`source` [TaskCompletionSource\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcompletionsource-1)<br/>

`task` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

`value` T<br/>

### **TrySetFromTask\<T\>(TaskCompletionSource\<T\>, Task\<T\>)**

```csharp
public static void TrySetFromTask<T>(TaskCompletionSource<T> source, Task<T> task)
```

#### Type Parameters

`T`<br/>

#### Parameters

`source` [TaskCompletionSource\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcompletionsource-1)<br/>

`task` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
