---

title: RollingTimer

---

# RollingTimer

Namespace: MassTransit.Util

Thread safe timer that allows efficient restarts by rolling the due time further into the future.
 Will roll over once every 43~ days of continuous runtime without a restart.

```csharp
public class RollingTimer : IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RollingTimer](../masstransit-util/rollingtimer)<br/>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **Triggered**

```csharp
public bool Triggered { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **RollingTimer(TimerCallback, TimeSpan, Object)**

```csharp
public RollingTimer(TimerCallback callback, TimeSpan timeout, object state)
```

#### Parameters

`callback` [TimerCallback](https://learn.microsoft.com/en-us/dotnet/api/system.threading.timercallback)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`state` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

## Methods

### **Dispose()**

```csharp
public void Dispose()
```

### **Start()**

Creates a new timer and starts it.

```csharp
public void Start()
```

### **Stop()**

Stops and disposes the existing timer.

```csharp
public void Stop()
```

### **Restart(Nullable\<TimeSpan\>)**

Restarts the existing timer, creates and starts a new timer if it does not exist.

```csharp
public void Restart(Nullable<TimeSpan> timeout)
```

#### Parameters

`timeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
