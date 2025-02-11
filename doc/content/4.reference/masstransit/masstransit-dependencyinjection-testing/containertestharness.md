---

title: ContainerTestHarness

---

# ContainerTestHarness

Namespace: MassTransit.DependencyInjection.Testing

```csharp
public class ContainerTestHarness : ITestHarness, IBaseTestHarness, IAsyncDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ContainerTestHarness](../masstransit-dependencyinjection-testing/containertestharness)<br/>
Implements [ITestHarness](../masstransit-testing/itestharness), [IBaseTestHarness](../masstransit-testing/ibasetestharness), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **InactivityTask**

```csharp
public Task InactivityTask { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Consumed**

```csharp
public IReceivedMessageList Consumed { get; }
```

#### Property Value

[IReceivedMessageList](../masstransit-testing/ireceivedmessagelist)<br/>

### **Published**

```csharp
public IPublishedMessageList Published { get; }
```

#### Property Value

[IPublishedMessageList](../masstransit-testing/ipublishedmessagelist)<br/>

### **Sent**

```csharp
public ISentMessageList Sent { get; }
```

#### Property Value

[ISentMessageList](../masstransit-testing/isentmessagelist)<br/>

### **Scope**

```csharp
public IServiceScope Scope { get; }
```

#### Property Value

IServiceScope<br/>

### **Provider**

```csharp
public IServiceProvider Provider { get; }
```

#### Property Value

IServiceProvider<br/>

### **EndpointNameFormatter**

```csharp
public IEndpointNameFormatter EndpointNameFormatter { get; }
```

#### Property Value

[IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

### **Bus**

```csharp
public IBus Bus { get; }
```

#### Property Value

[IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

### **TestTimeout**

```csharp
public TimeSpan TestTimeout { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **TestInactivityTimeout**

```csharp
public TimeSpan TestInactivityTimeout { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **InactivityToken**

CancellationToken that is cancelled when the test inactivity timeout has elapsed with no bus activity

```csharp
public CancellationToken InactivityToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **CancellationToken**

CancellationToken that is canceled when the test is being aborted

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Constructors

### **ContainerTestHarness(IServiceProvider, IOptions\<TestHarnessOptions\>)**

```csharp
public ContainerTestHarness(IServiceProvider provider, IOptions<TestHarnessOptions> options)
```

#### Parameters

`provider` IServiceProvider<br/>

`options` IOptions\<TestHarnessOptions\><br/>

## Methods

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>

### **Cancel()**

```csharp
public void Cancel()
```

### **ForceInactive()**

```csharp
public void ForceInactive()
```

### **GetConsumerHarness\<T\>()**

```csharp
public IConsumerTestHarness<T> GetConsumerHarness<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IConsumerTestHarness\<T\>](../masstransit-testing/iconsumertestharness-1)<br/>

### **GetSagaHarness\<T\>()**

```csharp
public ISagaTestHarness<T> GetSagaHarness<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[ISagaTestHarness\<T\>](../masstransit-testing/isagatestharness-1)<br/>

### **GetSagaStateMachineHarness\<TStateMachine, T\>()**

```csharp
public ISagaStateMachineTestHarness<TStateMachine, T> GetSagaStateMachineHarness<TStateMachine, T>()
```

#### Type Parameters

`TStateMachine`<br/>

`T`<br/>

#### Returns

[ISagaStateMachineTestHarness\<TStateMachine, T\>](../masstransit-testing/isagastatemachinetestharness-2)<br/>

### **GetRequestClient\<T\>()**

```csharp
public IRequestClient<T> GetRequestClient<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IRequestClient\<T\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

### **GetConsumerEndpoint\<T\>()**

```csharp
public Task<ISendEndpoint> GetConsumerEndpoint<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetHandlerEndpoint\<T\>()**

```csharp
public Task<ISendEndpoint> GetHandlerEndpoint<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetConsumerAddress\<T\>()**

```csharp
public Uri GetConsumerAddress<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

Uri<br/>

### **GetHandlerAddress\<T\>()**

```csharp
public Uri GetHandlerAddress<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

Uri<br/>

### **GetSagaEndpoint\<T\>()**

```csharp
public Task<ISendEndpoint> GetSagaEndpoint<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetSagaAddress\<T\>()**

```csharp
public Uri GetSagaAddress<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

Uri<br/>

### **GetExecuteActivityEndpoint\<T, TArguments\>()**

```csharp
public Task<ISendEndpoint> GetExecuteActivityEndpoint<T, TArguments>()
```

#### Type Parameters

`T`<br/>

`TArguments`<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetExecuteActivityAddress\<T, TArguments\>()**

```csharp
public Uri GetExecuteActivityAddress<T, TArguments>()
```

#### Type Parameters

`T`<br/>

`TArguments`<br/>

#### Returns

Uri<br/>

### **Start()**

```csharp
public Task Start()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **GetTask\<T\>()**

```csharp
public TaskCompletionSource<T> GetTask<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[TaskCompletionSource\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcompletionsource-1)<br/>

### **PostCreate(IBus)**

```csharp
public void PostCreate(IBus bus)
```

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

### **PostStart(IBus)**

```csharp
public void PostStart(IBus bus)
```

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>
