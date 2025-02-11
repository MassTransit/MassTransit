---

title: TestConsumeMessageObserver<T>

---

# TestConsumeMessageObserver\<T\>

Namespace: MassTransit.Testing.Implementations

```csharp
public class TestConsumeMessageObserver<T> : IConsumeMessageObserver<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TestConsumeMessageObserver\<T\>](../masstransit-testing-implementations/testconsumemessageobserver-1)<br/>
Implements [IConsumeMessageObserver\<T\>](../../masstransit-abstractions/masstransit/iconsumemessageobserver-1)

## Properties

### **PreConsumed**

```csharp
public Task<T> PreConsumed { get; }
```

#### Property Value

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **PostConsumed**

```csharp
public Task<T> PostConsumed { get; }
```

#### Property Value

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ConsumeFaulted**

```csharp
public Task<T> ConsumeFaulted { get; }
```

#### Property Value

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Constructors

### **TestConsumeMessageObserver(TaskCompletionSource\<T\>, TaskCompletionSource\<T\>, TaskCompletionSource\<T\>)**

```csharp
public TestConsumeMessageObserver(TaskCompletionSource<T> preConsumed, TaskCompletionSource<T> postConsumed, TaskCompletionSource<T> consumeFaulted)
```

#### Parameters

`preConsumed` [TaskCompletionSource\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcompletionsource-1)<br/>

`postConsumed` [TaskCompletionSource\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcompletionsource-1)<br/>

`consumeFaulted` [TaskCompletionSource\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcompletionsource-1)<br/>
