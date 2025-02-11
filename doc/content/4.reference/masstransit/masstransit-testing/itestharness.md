---

title: ITestHarness

---

# ITestHarness

Namespace: MassTransit.Testing

```csharp
public interface ITestHarness : IBaseTestHarness
```

Implements [IBaseTestHarness](../masstransit-testing/ibasetestharness)

## Properties

### **Bus**

```csharp
public abstract IBus Bus { get; }
```

#### Property Value

[IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

### **Scope**

```csharp
public abstract IServiceScope Scope { get; }
```

#### Property Value

IServiceScope<br/>

### **Provider**

```csharp
public abstract IServiceProvider Provider { get; }
```

#### Property Value

IServiceProvider<br/>

### **EndpointNameFormatter**

```csharp
public abstract IEndpointNameFormatter EndpointNameFormatter { get; }
```

#### Property Value

[IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

## Methods

### **GetTask\<T\>()**

Returns a task completion source that is automatically canceled when the test is canceled

```csharp
TaskCompletionSource<T> GetTask<T>()
```

#### Type Parameters

`T`<br/>
The task type

#### Returns

[TaskCompletionSource\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcompletionsource-1)<br/>

### **GetConsumerHarness\<T\>()**

Gets the consumer harness for the specified consumer from the container. Consumer test
 harnesses are automatically added to the container when AddConsumer is used.

```csharp
IConsumerTestHarness<T> GetConsumerHarness<T>()
```

#### Type Parameters

`T`<br/>
The consumer type

#### Returns

[IConsumerTestHarness\<T\>](../masstransit-testing/iconsumertestharness-1)<br/>

### **GetSagaHarness\<T\>()**

Gets the saga harness for the specified saga from the container. Saga test
 harnesses are automatically added to the container when AddSaga is used.

```csharp
ISagaTestHarness<T> GetSagaHarness<T>()
```

#### Type Parameters

`T`<br/>
The saga type

#### Returns

[ISagaTestHarness\<T\>](../masstransit-testing/isagatestharness-1)<br/>

### **GetSagaStateMachineHarness\<TStateMachine, T\>()**

Gets the saga harness for the specified saga from the container. Saga test
 harnesses are automatically added to the container when AddSaga is used.

```csharp
ISagaStateMachineTestHarness<TStateMachine, T> GetSagaStateMachineHarness<TStateMachine, T>()
```

#### Type Parameters

`TStateMachine`<br/>
The state machine type

`T`<br/>
The saga type

#### Returns

[ISagaStateMachineTestHarness\<TStateMachine, T\>](../masstransit-testing/isagastatemachinetestharness-2)<br/>

### **GetRequestClient\<T\>()**

```csharp
IRequestClient<T> GetRequestClient<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IRequestClient\<T\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

### **GetConsumerEndpoint\<T\>()**

Use the endpoint name formatter to get the send endpoint for the consumer type

```csharp
Task<ISendEndpoint> GetConsumerEndpoint<T>()
```

#### Type Parameters

`T`<br/>
The consumer type

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetHandlerEndpoint\<T\>()**

Use the endpoint name formatter to get the send endpoint for the message handler by message type

```csharp
Task<ISendEndpoint> GetHandlerEndpoint<T>()
```

#### Type Parameters

`T`<br/>
The message type

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetConsumerAddress\<T\>()**

Returns the endpoint address for the specified consumer type

```csharp
Uri GetConsumerAddress<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

Uri<br/>

### **GetHandlerAddress\<T\>()**

Returns the endpoint address for the specified handler type

```csharp
Uri GetHandlerAddress<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

Uri<br/>

### **GetSagaEndpoint\<T\>()**

Use the endpoint name formatter to get the send endpoint for the saga type

```csharp
Task<ISendEndpoint> GetSagaEndpoint<T>()
```

#### Type Parameters

`T`<br/>
The saga type

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetSagaAddress\<T\>()**

Returns the endpoint address for the saga

```csharp
Uri GetSagaAddress<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

Uri<br/>

### **GetExecuteActivityEndpoint\<T, TArguments\>()**

Use the endpoint name formatter to get the execute send endpoint for the activity type

```csharp
Task<ISendEndpoint> GetExecuteActivityEndpoint<T, TArguments>()
```

#### Type Parameters

`T`<br/>
The activity type

`TArguments`<br/>
The argument type

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetExecuteActivityAddress\<T, TArguments\>()**

Returns the endpoint address for the execute activity

```csharp
Uri GetExecuteActivityAddress<T, TArguments>()
```

#### Type Parameters

`T`<br/>

`TArguments`<br/>

#### Returns

Uri<br/>

### **Start()**

```csharp
Task Start()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
