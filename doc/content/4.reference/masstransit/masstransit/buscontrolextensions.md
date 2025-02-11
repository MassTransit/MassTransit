---

title: BusControlExtensions

---

# BusControlExtensions

Namespace: MassTransit

```csharp
public static class BusControlExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BusControlExtensions](../masstransit/buscontrolextensions)

## Methods

### **Stop(IBusControl)**

Stop a bus, throwing an exception if the bus does not stop.
 It is a wrapper of the async method `StopAsync`

```csharp
public static void Stop(IBusControl busControl)
```

#### Parameters

`busControl` [IBusControl](../../masstransit-abstractions/masstransit/ibuscontrol)<br/>
The bus handle

### **Start(IBusControl)**

Starts a bus, throwing an exception if the bus does not start
 It is a wrapper of the async method `StartAsync`

```csharp
public static void Start(IBusControl busControl)
```

#### Parameters

`busControl` [IBusControl](../../masstransit-abstractions/masstransit/ibuscontrol)<br/>
The bus handle

### **Stop(IBusControl, TimeSpan)**

Stop a bus, throwing an exception if the bus does not stop in the specified timeout

```csharp
public static void Stop(IBusControl bus, TimeSpan stopTimeout)
```

#### Parameters

`bus` [IBusControl](../../masstransit-abstractions/masstransit/ibuscontrol)<br/>
The bus handle

`stopTimeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The wait time before throwing an exception

### **Start(IBusControl, TimeSpan)**

Start a bus, throwing an exception if the bus does not start in the specified timeout

```csharp
public static void Start(IBusControl bus, TimeSpan startTimeout)
```

#### Parameters

`bus` [IBusControl](../../masstransit-abstractions/masstransit/ibuscontrol)<br/>
The bus handle

`startTimeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The wait time before throwing an exception

### **StartAsync(IBusControl, TimeSpan)**

Start a bus, throwing an exception if the bus does not start in the specified timeout

```csharp
public static Task<BusHandle> StartAsync(IBusControl bus, TimeSpan startTimeout)
```

#### Parameters

`bus` [IBusControl](../../masstransit-abstractions/masstransit/ibuscontrol)<br/>
The bus handle

`startTimeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The wait time before throwing an exception

#### Returns

[Task\<BusHandle\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **StopAsync(IBusControl, TimeSpan)**

Stop a bus, throwing an exception if the bus does not stop in the specified timeout

```csharp
public static Task StopAsync(IBusControl bus, TimeSpan stopTimeout)
```

#### Parameters

`bus` [IBusControl](../../masstransit-abstractions/masstransit/ibuscontrol)<br/>
The bus handle

`stopTimeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The wait time before throwing an exception

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **DeployAsync(IBusControl, CancellationToken)**

This can be used to start and stop the bus when configured in a deploy topology only scenario. No messages should be consumed by it.

```csharp
public static Task DeployAsync(IBusControl bus, CancellationToken cancellationToken)
```

#### Parameters

`bus` [IBusControl](../../masstransit-abstractions/masstransit/ibuscontrol)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

#### Exceptions

[ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br/>
