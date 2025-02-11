---

title: BusHandleExtensions

---

# BusHandleExtensions

Namespace: MassTransit

```csharp
public static class BusHandleExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BusHandleExtensions](../masstransit/bushandleextensions)

## Methods

### **Stop(BusHandle)**

Stop a bus, throwing an exception if the bus does not stop

```csharp
public static void Stop(BusHandle handle)
```

#### Parameters

`handle` [BusHandle](../../masstransit-abstractions/masstransit/bushandle)<br/>
The bus handle

### **Stop(BusHandle, TimeSpan)**

Stop a bus, throwing an exception if the bus does not stop in the specified timeout

```csharp
public static void Stop(BusHandle handle, TimeSpan stopTimeout)
```

#### Parameters

`handle` [BusHandle](../../masstransit-abstractions/masstransit/bushandle)<br/>
The bus handle

`stopTimeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The wait time before throwing an exception

### **StopAsync(BusHandle, TimeSpan)**

Stop a bus, throwing an exception if the bus does not stop in the specified timeout

```csharp
public static Task StopAsync(BusHandle handle, TimeSpan stopTimeout)
```

#### Parameters

`handle` [BusHandle](../../masstransit-abstractions/masstransit/bushandle)<br/>
The bus handle

`stopTimeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The wait time before throwing an exception

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
