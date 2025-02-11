---

title: AsyncTestHarness

---

# AsyncTestHarness

Namespace: MassTransit.Testing

```csharp
public abstract class AsyncTestHarness : IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncTestHarness](../masstransit-testing/asynctestharness)<br/>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **TestCancelledTask**

Task that is canceled when the test is aborted, for continueWith usage

```csharp
public Task TestCancelledTask { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **TestCancellationToken**

CancellationToken that is canceled when the test is being aborted

```csharp
public CancellationToken TestCancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **InactivityTask**

Task that is completed when the bus inactivity timeout has elapsed with no bus activity

```csharp
public Task InactivityTask { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **InactivityToken**

CancellationToken that is cancelled when the test inactivity timeout has elapsed with no bus activity

```csharp
public CancellationToken InactivityToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **InactivityObserver**

```csharp
public IInactivityObserver InactivityObserver { get; }
```

#### Property Value

[IInactivityObserver](../masstransit-testing-implementations/iinactivityobserver)<br/>

### **TestTimeout**

Timeout for the test, used for any delay timers

```csharp
public TimeSpan TestTimeout { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **TestInactivityTimeout**

Timeout specifying the elapsed time with no bus activity after which the test could be completed

```csharp
public TimeSpan TestInactivityTimeout { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Methods

### **Dispose()**

```csharp
public void Dispose()
```

### **Cancel()**

Forces the test to be cancelled, aborting any awaiting tasks

```csharp
public void Cancel()
```

### **ForceInactive()**

```csharp
public void ForceInactive()
```

### **GetTask\<T\>()**

Returns a task completion that is automatically canceled when the test is canceled

```csharp
public TaskCompletionSource<T> GetTask<T>()
```

#### Type Parameters

`T`<br/>
The task type

#### Returns

[TaskCompletionSource\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcompletionsource-1)<br/>

### **GetConsumeObserver\<T\>()**

```csharp
public TestConsumeMessageObserver<T> GetConsumeObserver<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[TestConsumeMessageObserver\<T\>](../masstransit-testing-implementations/testconsumemessageobserver-1)<br/>

### **GetConsumeObserver()**

```csharp
public TestConsumeObserver GetConsumeObserver()
```

#### Returns

[TestConsumeObserver](../masstransit-testing-implementations/testconsumeobserver)<br/>
