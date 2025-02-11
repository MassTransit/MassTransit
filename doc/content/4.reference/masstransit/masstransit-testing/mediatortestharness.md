---

title: MediatorTestHarness

---

# MediatorTestHarness

Namespace: MassTransit.Testing

```csharp
public class MediatorTestHarness : AsyncTestHarness, IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncTestHarness](../masstransit-testing/asynctestharness) → [MediatorTestHarness](../masstransit-testing/mediatortestharness)<br/>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **Mediator**

```csharp
public IMediator Mediator { get; private set; }
```

#### Property Value

[IMediator](../../masstransit-abstractions/masstransit-mediator/imediator)<br/>

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

## Constructors

### **MediatorTestHarness()**

```csharp
public MediatorTestHarness()
```

## Methods

### **Start()**

```csharp
public Task Start()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ConfigureMediator(IMediatorConfigurator)**

```csharp
protected void ConfigureMediator(IMediatorConfigurator configurator)
```

#### Parameters

`configurator` [IMediatorConfigurator](../masstransit/imediatorconfigurator)<br/>

### **CreateRequestClient\<TRequest\>()**

```csharp
public IRequestClient<TRequest> CreateRequestClient<TRequest>()
```

#### Type Parameters

`TRequest`<br/>

#### Returns

[IRequestClient\<TRequest\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

## Events

### **OnConfigureMediator**

```csharp
public event Action<IMediatorConfigurator> OnConfigureMediator;
```
