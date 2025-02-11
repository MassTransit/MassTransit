---

title: TaskInitializerExtensions

---

# TaskInitializerExtensions

Namespace: MassTransit.Initializers

```csharp
public static class TaskInitializerExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TaskInitializerExtensions](../masstransit-initializers/taskinitializerextensions)

## Methods

### **Select\<T\>(Task\<T\>, Func\<T, String\>)**

Awaits the task and calls the selector to return a string property of the result

```csharp
public static Task<string> Select<T>(Task<T> task, Func<T, string> selector)
```

#### Type Parameters

`T`<br/>

#### Parameters

`task` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`selector` [Func\<T, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[Task\<String\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Select\<T\>(Task\<T\>, Func\<T, String\>, String)**

Awaits the task and calls the selector to return a string property of the result

```csharp
public static Task<string> Select<T>(Task<T> task, Func<T, string> selector, string defaultValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`task` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`selector` [Func\<T, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`defaultValue` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Task\<String\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Select\<T\>(Task\<T\>, Func\<T, String\>, Func\<String\>)**

Awaits the task and calls the selector to return a string property of the result

```csharp
public static Task<string> Select<T>(Task<T> task, Func<T, string> selector, Func<string> getDefaultValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`task` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`selector` [Func\<T, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`getDefaultValue` [Func\<String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

#### Returns

[Task\<String\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Select\<T\>(Task\<T\>, Func\<T, String\>, Func\<Task\<String\>\>)**

Awaits the task and calls the selector to return a string property of the result

```csharp
public static Task<string> Select<T>(Task<T> task, Func<T, string> selector, Func<Task<string>> getDefaultValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`task` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`selector` [Func\<T, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`getDefaultValue` [Func\<Task\<String\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

#### Returns

[Task\<String\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Select\<T, TResult\>(Task\<T\>, Func\<T, TResult\>)**

Awaits the task and calls the selector to return a string property of the result

```csharp
public static Task<TResult> Select<T, TResult>(Task<T> task, Func<T, TResult> selector)
```

#### Type Parameters

`T`<br/>

`TResult`<br/>

#### Parameters

`task` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`selector` [Func\<T, TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[Task\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Select\<T, TResult\>(Task\<T\>, Func\<T, TResult\>, TResult)**

Awaits the task and calls the selector to return a TResult property of the result

```csharp
public static Task<TResult> Select<T, TResult>(Task<T> task, Func<T, TResult> selector, TResult defaultValue)
```

#### Type Parameters

`T`<br/>

`TResult`<br/>

#### Parameters

`task` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`selector` [Func\<T, TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`defaultValue` TResult<br/>

#### Returns

[Task\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Select\<T, TResult\>(Task\<T\>, Func\<T, Nullable\<TResult\>\>, TResult)**

Awaits the task and calls the selector to return a TResult property of the result

```csharp
public static Task<TResult> Select<T, TResult>(Task<T> task, Func<T, Nullable<TResult>> selector, TResult defaultValue)
```

#### Type Parameters

`T`<br/>

`TResult`<br/>

#### Parameters

`task` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`selector` [Func\<T, Nullable\<TResult\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`defaultValue` TResult<br/>

#### Returns

[Task\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Select\<T, TResult\>(Task\<T\>, Func\<T, TResult\>, Func\<TResult\>)**

Awaits the task and calls the selector to return a TResult property of the result

```csharp
public static Task<TResult> Select<T, TResult>(Task<T> task, Func<T, TResult> selector, Func<TResult> getDefaultValue)
```

#### Type Parameters

`T`<br/>

`TResult`<br/>

#### Parameters

`task` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`selector` [Func\<T, TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`getDefaultValue` [Func\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

#### Returns

[Task\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Select\<T, TResult\>(Task\<T\>, Func\<T, Nullable\<TResult\>\>, Func\<TResult\>)**

Awaits the task and calls the selector to return a TResult property of the result

```csharp
public static Task<TResult> Select<T, TResult>(Task<T> task, Func<T, Nullable<TResult>> selector, Func<TResult> getDefaultValue)
```

#### Type Parameters

`T`<br/>

`TResult`<br/>

#### Parameters

`task` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`selector` [Func\<T, Nullable\<TResult\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`getDefaultValue` [Func\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

#### Returns

[Task\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Select\<T, TResult\>(Task\<T\>, Func\<T, TResult\>, Func\<Task\<TResult\>\>)**

Awaits the task and calls the selector to return a TResult property of the result

```csharp
public static Task<TResult> Select<T, TResult>(Task<T> task, Func<T, TResult> selector, Func<Task<TResult>> getDefaultValue)
```

#### Type Parameters

`T`<br/>

`TResult`<br/>

#### Parameters

`task` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`selector` [Func\<T, TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`getDefaultValue` [Func\<Task\<TResult\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

#### Returns

[Task\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Select\<T, TResult\>(Task\<T\>, Func\<T, Nullable\<TResult\>\>, Func\<Task\<TResult\>\>)**

Awaits the task and calls the selector to return a TResult property of the result

```csharp
public static Task<TResult> Select<T, TResult>(Task<T> task, Func<T, Nullable<TResult>> selector, Func<Task<TResult>> getDefaultValue)
```

#### Type Parameters

`T`<br/>

`TResult`<br/>

#### Parameters

`task` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`selector` [Func\<T, Nullable\<TResult\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`getDefaultValue` [Func\<Task\<TResult\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

#### Returns

[Task\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
