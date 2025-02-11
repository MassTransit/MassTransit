---

title: InMemoryTestHarness

---

# InMemoryTestHarness

Namespace: MassTransit.Testing

```csharp
public class InMemoryTestHarness : BusTestHarness, IDisposable, IBaseTestHarness
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncTestHarness](../masstransit-testing/asynctestharness) → [BusTestHarness](../masstransit-testing/bustestharness) → [InMemoryTestHarness](../masstransit-testing/inmemorytestharness)<br/>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable), [IBaseTestHarness](../masstransit-testing/ibasetestharness)

## Properties

### **BaseAddress**

```csharp
public Uri BaseAddress { get; }
```

#### Property Value

Uri<br/>

### **InputQueueAddress**

```csharp
public Uri InputQueueAddress { get; }
```

#### Property Value

Uri<br/>

### **InputQueueName**

```csharp
public string InputQueueName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **BusControl**

```csharp
public IBusControl BusControl { get; }
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

### **BusSendEndpoint**

The send endpoint for the default bus endpoint

```csharp
public ISendEndpoint BusSendEndpoint { get; }
```

#### Property Value

[ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint)<br/>

### **InputQueueSendEndpoint**

The send endpoint for the input queue receive endpoint

```csharp
public ISendEndpoint InputQueueSendEndpoint { get; }
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

## Constructors

### **InMemoryTestHarness(String)**

```csharp
public InMemoryTestHarness(string virtualHost)
```

#### Parameters

`virtualHost` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **InMemoryTestHarness(String, IEnumerable\<IBusInstanceSpecification\>)**

```csharp
public InMemoryTestHarness(string virtualHost, IEnumerable<IBusInstanceSpecification> specifications)
```

#### Parameters

`virtualHost` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`specifications` [IEnumerable\<IBusInstanceSpecification\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Methods

### **ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator)**

```csharp
protected void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
```

#### Parameters

`configurator` [IInMemoryBusFactoryConfigurator](../masstransit/iinmemorybusfactoryconfigurator)<br/>

### **ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator)**

```csharp
protected void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
```

#### Parameters

`configurator` [IInMemoryReceiveEndpointConfigurator](../masstransit/iinmemoryreceiveendpointconfigurator)<br/>

### **InMemoryBusConfigured(IInMemoryBusFactoryConfigurator)**

```csharp
protected void InMemoryBusConfigured(IInMemoryBusFactoryConfigurator configurator)
```

#### Parameters

`configurator` [IInMemoryBusFactoryConfigurator](../masstransit/iinmemorybusfactoryconfigurator)<br/>

### **ConnectRequestClient\<TRequest\>()**

```csharp
public Task<IRequestClient<TRequest>> ConnectRequestClient<TRequest>()
```

#### Type Parameters

`TRequest`<br/>

#### Returns

[Task\<IRequestClient\<TRequest\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ConnectRequestClient\<TRequest\>(Uri)**

```csharp
public Task<IRequestClient<TRequest>> ConnectRequestClient<TRequest>(Uri destinationAddress)
```

#### Type Parameters

`TRequest`<br/>

#### Parameters

`destinationAddress` Uri<br/>

#### Returns

[Task\<IRequestClient\<TRequest\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **CreateBus()**

```csharp
protected Task<IBusControl> CreateBus()
```

#### Returns

[Task\<IBusControl\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Events

### **OnConfigureInMemoryBus**

```csharp
public event Action<IInMemoryBusFactoryConfigurator> OnConfigureInMemoryBus;
```

### **OnConfigureInMemoryReceiveEndpoint**

```csharp
public event Action<IInMemoryReceiveEndpointConfigurator> OnConfigureInMemoryReceiveEndpoint;
```

### **OnInMemoryBusConfigured**

```csharp
public event Action<IInMemoryBusFactoryConfigurator> OnInMemoryBusConfigured;
```

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
