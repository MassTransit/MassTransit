---

title: DisposeAsyncExtensions

---

# DisposeAsyncExtensions

Namespace: MassTransit.Util

```csharp
public static class DisposeAsyncExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DisposeAsyncExtensions](../masstransit-util/disposeasyncextensions)

## Methods

### **DisposeAsync\<T\>(Exception, Func\<Task\>)**

Invoke the dispose callback, and then rethrow the exception

```csharp
public static ValueTask<T> DisposeAsync<T>(Exception exception, Func<Task> disposeCallback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`disposeCallback` [Func\<Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

#### Returns

[ValueTask\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1)<br/>

#### Exceptions

T:MassTransit.MassTransitException<br/>

### **DisposeAsync\<T\>(Exception, Func\<ValueTask\>)**

Invoke the dispose callback, and then rethrow the exception

```csharp
public static ValueTask<T> DisposeAsync<T>(Exception exception, Func<ValueTask> disposeCallback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`disposeCallback` [Func\<ValueTask\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

#### Returns

[ValueTask\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1)<br/>

#### Exceptions

T:MassTransit.MassTransitException<br/>
