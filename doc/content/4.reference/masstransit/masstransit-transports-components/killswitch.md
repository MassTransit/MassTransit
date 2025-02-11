---

title: KillSwitch

---

# KillSwitch

Namespace: MassTransit.Transports.Components

The KillSwitch monitors a receive endpoint, stopping and restarting as necessary in the presence of exceptions.

```csharp
public class KillSwitch : IKillSwitch, IReceiveEndpointObserver, IConsumeObserver, IActivityObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [KillSwitch](../masstransit-transports-components/killswitch)<br/>
Implements [IKillSwitch](../masstransit-transports-components/ikillswitch), [IReceiveEndpointObserver](../../masstransit-abstractions/masstransit/ireceiveendpointobserver), [IConsumeObserver](../../masstransit-abstractions/masstransit/iconsumeobserver), [IActivityObserver](../../masstransit-abstractions/masstransit/iactivityobserver)

## Properties

### **ActivationThreshold**

```csharp
public int ActivationThreshold { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **TripThreshold**

```csharp
public int TripThreshold { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **TrackingPeriod**

```csharp
public TimeSpan TrackingPeriod { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **RestartTimeout**

```csharp
public TimeSpan RestartTimeout { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **ExceptionFilter**

```csharp
public IExceptionFilter ExceptionFilter { get; }
```

#### Property Value

[IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

## Constructors

### **KillSwitch(KillSwitchOptions)**

```csharp
public KillSwitch(KillSwitchOptions options)
```

#### Parameters

`options` [KillSwitchOptions](../masstransit-transports-components/killswitchoptions)<br/>

## Methods

### **PreExecute\<TActivity, TArguments\>(ExecuteActivityContext\<TActivity, TArguments\>)**

```csharp
public Task PreExecute<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`context` [ExecuteActivityContext\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/executeactivitycontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostExecute\<TActivity, TArguments\>(ExecuteActivityContext\<TActivity, TArguments\>)**

```csharp
public Task PostExecute<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`context` [ExecuteActivityContext\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/executeactivitycontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ExecuteFault\<TActivity, TArguments\>(ExecuteActivityContext\<TActivity, TArguments\>, Exception)**

```csharp
public Task ExecuteFault<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context, Exception exception)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`context` [ExecuteActivityContext\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/executeactivitycontext-2)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PreCompensate\<TActivity, TLog\>(CompensateActivityContext\<TActivity, TLog\>)**

```csharp
public Task PreCompensate<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`context` [CompensateActivityContext\<TActivity, TLog\>](../../masstransit-abstractions/masstransit/compensateactivitycontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostCompensate\<TActivity, TLog\>(CompensateActivityContext\<TActivity, TLog\>)**

```csharp
public Task PostCompensate<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`context` [CompensateActivityContext\<TActivity, TLog\>](../../masstransit-abstractions/masstransit/compensateactivitycontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CompensateFail\<TActivity, TLog\>(CompensateActivityContext\<TActivity, TLog\>, Exception)**

```csharp
public Task CompensateFail<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context, Exception exception)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`context` [CompensateActivityContext\<TActivity, TLog\>](../../masstransit-abstractions/masstransit/compensateactivitycontext-2)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PreConsume\<T\>(ConsumeContext\<T\>)**

```csharp
public Task PreConsume<T>(ConsumeContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostConsume\<T\>(ConsumeContext\<T\>)**

```csharp
public Task PostConsume<T>(ConsumeContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ConsumeFault\<T\>(ConsumeContext\<T\>, Exception)**

```csharp
public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Started(IKillSwitchState)**

```csharp
public void Started(IKillSwitchState previousState)
```

#### Parameters

`previousState` [IKillSwitchState](../masstransit-transports-components/ikillswitchstate)<br/>

### **Restart(Exception, IKillSwitchState)**

```csharp
public void Restart(Exception exception, IKillSwitchState previousState)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`previousState` [IKillSwitchState](../masstransit-transports-components/ikillswitchstate)<br/>

### **Stop(Exception, IKillSwitchState)**

```csharp
public void Stop(Exception exception, IKillSwitchState previousState)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`previousState` [IKillSwitchState](../masstransit-transports-components/ikillswitchstate)<br/>

### **Ready(ReceiveEndpointReady)**

```csharp
public Task Ready(ReceiveEndpointReady ready)
```

#### Parameters

`ready` [ReceiveEndpointReady](../../masstransit-abstractions/masstransit/receiveendpointready)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Stopping(ReceiveEndpointStopping)**

```csharp
public Task Stopping(ReceiveEndpointStopping stopping)
```

#### Parameters

`stopping` [ReceiveEndpointStopping](../../masstransit-abstractions/masstransit/receiveendpointstopping)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Completed(ReceiveEndpointCompleted)**

```csharp
public Task Completed(ReceiveEndpointCompleted completed)
```

#### Parameters

`completed` [ReceiveEndpointCompleted](../../masstransit-abstractions/masstransit/receiveendpointcompleted)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted(ReceiveEndpointFaulted)**

```csharp
public Task Faulted(ReceiveEndpointFaulted faulted)
```

#### Parameters

`faulted` [ReceiveEndpointFaulted](../../masstransit-abstractions/masstransit/receiveendpointfaulted)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
