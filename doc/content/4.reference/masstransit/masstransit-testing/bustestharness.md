---

title: BusTestHarness

---

# BusTestHarness

Namespace: MassTransit.Testing

A bus text fixture includes a single bus instance with one or more receiving endpoints.

```csharp
public abstract class BusTestHarness : AsyncTestHarness, IDisposable, IBaseTestHarness
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncTestHarness](../masstransit-testing/asynctestharness) → [BusTestHarness](../masstransit-testing/bustestharness)<br/>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable), [IBaseTestHarness](../masstransit-testing/ibasetestharness)

## Properties

### **BusControl**

```csharp
public IBusControl BusControl { get; private set; }
```

#### Property Value

[IBusControl](../../masstransit-abstractions/masstransit/ibuscontrol)<br/>

### **BusAddress**

The address of the default bus endpoint, used as the SourceAddress for requests and published messages

```csharp
public Uri BusAddress { get; }
```

#### Property Value

Uri<br/>

### **InputQueueName**

The name of the input queue (for the default receive endpoint)

```csharp
public abstract string InputQueueName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **InputQueueAddress**

The address of the input queue receive endpoint

```csharp
public abstract Uri InputQueueAddress { get; }
```

#### Property Value

Uri<br/>

### **BusSendEndpoint**

The send endpoint for the default bus endpoint

```csharp
public ISendEndpoint BusSendEndpoint { get; private set; }
```

#### Property Value

[ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint)<br/>

### **InputQueueSendEndpoint**

The send endpoint for the input queue receive endpoint

```csharp
public ISendEndpoint InputQueueSendEndpoint { get; private set; }
```

#### Property Value

[ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint)<br/>

### **Bus**

```csharp
public IBus Bus { get; }
```

#### Property Value

[IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

### **Sent**

```csharp
public ISentMessageList Sent { get; }
```

#### Property Value

[ISentMessageList](../masstransit-testing/isentmessagelist)<br/>

### **CancellationToken**

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

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

### **CreateBus()**

```csharp
protected abstract Task<IBusControl> CreateBus()
```

#### Returns

[Task\<IBusControl\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **CreateRequestClient\<TRequest\>()**

```csharp
public IRequestClient<TRequest> CreateRequestClient<TRequest>()
```

#### Type Parameters

`TRequest`<br/>

#### Returns

[IRequestClient\<TRequest\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

### **CreateRequestClient\<TRequest\>(Uri)**

```csharp
public IRequestClient<TRequest> CreateRequestClient<TRequest>(Uri destinationAddress)
```

#### Type Parameters

`TRequest`<br/>

#### Parameters

`destinationAddress` Uri<br/>

#### Returns

[IRequestClient\<TRequest\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

### **ConnectObservers(IBus)**

```csharp
protected void ConnectObservers(IBus bus)
```

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

### **ConfigureBus(IBusFactoryConfigurator)**

```csharp
protected void ConfigureBus(IBusFactoryConfigurator configurator)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

### **ConfigureReceiveEndpoint(IReceiveEndpointConfigurator)**

```csharp
protected void ConfigureReceiveEndpoint(IReceiveEndpointConfigurator configurator)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

### **BusConfigured(IBusFactoryConfigurator)**

```csharp
protected void BusConfigured(IBusFactoryConfigurator configurator)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

### **Start(CancellationToken)**

```csharp
public Task Start(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Stop()**

```csharp
public Task Stop()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Clean()**

```csharp
public Task Clean()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **GetSendEndpoint(Uri)**

```csharp
public Task<ISendEndpoint> GetSendEndpoint(Uri address)
```

#### Parameters

`address` Uri<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SubscribeHandler\<T\>()**

Subscribes a message handler to the bus, which is disconnected after the message
 is received.

```csharp
public Task<ConsumeContext<T>> SubscribeHandler<T>()
```

#### Type Parameters

`T`<br/>
The message type

#### Returns

[Task\<ConsumeContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
An awaitable task completed when the message is received

### **SubscribeHandler\<T\>(Func\<ConsumeContext\<T\>, Boolean\>)**

Subscribes a message handler to the bus, which is disconnected after the message
 is received.

```csharp
public Task<ConsumeContext<T>> SubscribeHandler<T>(Func<ConsumeContext<T>, bool> filter)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`filter` [Func\<ConsumeContext\<T\>, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
A filter that only completes the task if filter is true

#### Returns

[Task\<ConsumeContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
An awaitable task completed when the message is received

### **Handled\<T\>(IReceiveEndpointConfigurator)**

Registers a handler on the receive endpoint that is cancelled when the test is canceled
 and completed when the message is received.

```csharp
public Task<ConsumeContext<T>> Handled<T>(IReceiveEndpointConfigurator configurator)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
The endpoint configurator

#### Returns

[Task\<ConsumeContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Handled\<T\>(IReceiveEndpointConfigurator, Func\<ConsumeContext\<T\>, Boolean\>)**

Registers a handler on the receive endpoint that is cancelled when the test is canceled
 and completed when the message is received.

```csharp
public Task<ConsumeContext<T>> Handled<T>(IReceiveEndpointConfigurator configurator, Func<ConsumeContext<T>, bool> filter)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
The endpoint configurator

`filter` [Func\<ConsumeContext\<T\>, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Filter the messages based on the handled consume context

#### Returns

[Task\<ConsumeContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Handled\<T\>(IReceiveEndpointConfigurator, Int32)**

Registers a handler on the receive endpoint that is cancelled when the test is canceled
 and completed when the message is received.

```csharp
public Task<ConsumeContext<T>> Handled<T>(IReceiveEndpointConfigurator configurator, int expectedCount)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
The endpoint configurator

`expectedCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The expected number of messages

#### Returns

[Task\<ConsumeContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Handler\<T\>(IReceiveEndpointConfigurator, MessageHandler\<T\>)**

Registers a handler on the receive endpoint that is completed after the specified handler is
 executed and canceled if the test is canceled.

```csharp
public Task<ConsumeContext<T>> Handler<T>(IReceiveEndpointConfigurator configurator, MessageHandler<T> handler)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`handler` [MessageHandler\<T\>](../../masstransit-abstractions/masstransit/messagehandler-1)<br/>

#### Returns

[Task\<ConsumeContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **HandledByConsumer\<T\>(IReceiveEndpointConfigurator)**

Registers a consumer on the receive endpoint that is cancelled when the test is canceled
 and completed when the message is received.

```csharp
public Task<ConsumeContext<T>> HandledByConsumer<T>(IReceiveEndpointConfigurator configurator)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
The endpoint configurator

#### Returns

[Task\<ConsumeContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Events

### **PreCreateBus**

```csharp
public event Action<BusTestHarness> PreCreateBus;
```

### **OnConfigureReceiveEndpoint**

```csharp
public event Action<IReceiveEndpointConfigurator> OnConfigureReceiveEndpoint;
```

### **OnConfigureBus**

```csharp
public event Action<IBusFactoryConfigurator> OnConfigureBus;
```

### **OnBusConfigured**

```csharp
public event Action<IBusFactoryConfigurator> OnBusConfigured;
```

### **OnConnectObservers**

```csharp
public event Action<IBus> OnConnectObservers;
```
