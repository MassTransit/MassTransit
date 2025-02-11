---

title: IKillSwitch

---

# IKillSwitch

Namespace: MassTransit.Transports.Components

```csharp
public interface IKillSwitch
```

## Properties

### **LogContext**

```csharp
public abstract ILogContext LogContext { get; }
```

#### Property Value

[ILogContext](../masstransit-logging/ilogcontext)<br/>

### **ActivationThreshold**

The minimum number of attempts before the breaker can possibly trip

```csharp
public abstract int ActivationThreshold { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **TripThreshold**

The number of failures before opening the circuit breaker

```csharp
public abstract int TripThreshold { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **TrackingPeriod**

Window duration before attempt/success/failure counts are reset

```csharp
public abstract TimeSpan TrackingPeriod { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **RestartTimeout**

The wait time before restarting

```csharp
public abstract TimeSpan RestartTimeout { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **ExceptionFilter**

Matches the supported exceptions for the kill switch

```csharp
public abstract IExceptionFilter ExceptionFilter { get; }
```

#### Property Value

[IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

## Methods

### **Stop(Exception, IKillSwitchState)**

Stop for the restart timeout

```csharp
void Stop(Exception exception, IKillSwitchState previousState)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
The exception to return when the circuit breaker is accessed

`previousState` [IKillSwitchState](../masstransit-transports-components/ikillswitchstate)<br/>

### **Restart(Exception, IKillSwitchState)**

Restart, monitoring exception rates until they stabilize

```csharp
void Restart(Exception exception, IKillSwitchState previousState)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`previousState` [IKillSwitchState](../masstransit-transports-components/ikillswitchstate)<br/>

### **Started(IKillSwitchState)**

Transition to the Started state, where exception rates are below the trip threshold

```csharp
void Started(IKillSwitchState previousState)
```

#### Parameters

`previousState` [IKillSwitchState](../masstransit-transports-components/ikillswitchstate)<br/>
