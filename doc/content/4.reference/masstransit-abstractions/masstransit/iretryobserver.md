---

title: IRetryObserver

---

# IRetryObserver

Namespace: MassTransit

```csharp
public interface IRetryObserver
```

## Methods

### **PostCreate\<T\>(RetryPolicyContext\<T\>)**

Called before a message is dispatched to any consumers

```csharp
Task PostCreate<T>(RetryPolicyContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [RetryPolicyContext\<T\>](../masstransit/retrypolicycontext-1)<br/>
The consume context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostFault\<T\>(RetryContext\<T\>)**

Called after a fault has occurred, but will be retried

```csharp
Task PostFault<T>(RetryContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [RetryContext\<T\>](../masstransit/retrycontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PreRetry\<T\>(RetryContext\<T\>)**

Called immediately before an exception will be retried

```csharp
Task PreRetry<T>(RetryContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [RetryContext\<T\>](../masstransit/retrycontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RetryFault\<T\>(RetryContext\<T\>)**

Called when the retry filter is no longer going to retry, and the context is faulted.

```csharp
Task RetryFault<T>(RetryContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [RetryContext\<T\>](../masstransit/retrycontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RetryComplete\<T\>(RetryContext\<T\>)**

Called when the retry filter retried at least once, and the context completed successfully.

```csharp
Task RetryComplete<T>(RetryContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [RetryContext\<T\>](../masstransit/retrycontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
