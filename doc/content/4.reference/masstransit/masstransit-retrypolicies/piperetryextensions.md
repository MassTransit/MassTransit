---

title: PipeRetryExtensions

---

# PipeRetryExtensions

Namespace: MassTransit.RetryPolicies

```csharp
public static class PipeRetryExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PipeRetryExtensions](../masstransit-retrypolicies/piperetryextensions)

## Methods

### **Retry(IRetryPolicy, Func\<Task\>, CancellationToken)**

```csharp
public static Task Retry(IRetryPolicy retryPolicy, Func<Task> retryMethod, CancellationToken cancellationToken)
```

#### Parameters

`retryPolicy` [IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

`retryMethod` [Func\<Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Retry(IRetryPolicy, Func\<Task\>, Boolean, CancellationToken)**

```csharp
public static Task Retry(IRetryPolicy retryPolicy, Func<Task> retryMethod, bool log, CancellationToken cancellationToken)
```

#### Parameters

`retryPolicy` [IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

`retryMethod` [Func\<Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`log` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Retry\<T\>(IRetryPolicy, Func\<Task\<T\>\>, CancellationToken)**

```csharp
public static Task<T> Retry<T>(IRetryPolicy retryPolicy, Func<Task<T>> retryMethod, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`retryPolicy` [IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

`retryMethod` [Func\<Task\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Retry\<T\>(IRetryPolicy, Func\<Task\<T\>\>, Boolean, CancellationToken)**

```csharp
public static Task<T> Retry<T>(IRetryPolicy retryPolicy, Func<Task<T>> retryMethod, bool log, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`retryPolicy` [IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

`retryMethod` [Func\<Task\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`log` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
