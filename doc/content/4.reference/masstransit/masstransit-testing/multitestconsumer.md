---

title: MultiTestConsumer

---

# MultiTestConsumer

Namespace: MassTransit.Testing

```csharp
public class MultiTestConsumer
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MultiTestConsumer](../masstransit-testing/multitestconsumer)

## Properties

### **Received**

```csharp
public IReceivedMessageList Received { get; }
```

#### Property Value

[IReceivedMessageList](../masstransit-testing/ireceivedmessagelist)<br/>

### **Timeout**

```csharp
public TimeSpan Timeout { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Constructors

### **MultiTestConsumer(TimeSpan, CancellationToken)**

```csharp
public MultiTestConsumer(TimeSpan timeout, CancellationToken testCompleted)
```

#### Parameters

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`testCompleted` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **Consume\<T\>()**

```csharp
public ReceivedMessageList<T> Consume<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[ReceivedMessageList\<T\>](../masstransit-testing/receivedmessagelist-1)<br/>

### **Fault\<T\>()**

```csharp
public ReceivedMessageList<T> Fault<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[ReceivedMessageList\<T\>](../masstransit-testing/receivedmessagelist-1)<br/>

### **Connect(IConsumePipeConnector)**

```csharp
public ConnectHandle Connect(IConsumePipeConnector bus)
```

#### Parameters

`bus` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **Configure(IReceiveEndpointConfigurator)**

```csharp
public void Configure(IReceiveEndpointConfigurator configurator)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
