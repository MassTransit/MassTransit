---

title: BusControlHealthExtensions

---

# BusControlHealthExtensions

Namespace: MassTransit

```csharp
public static class BusControlHealthExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BusControlHealthExtensions](../masstransit/buscontrolhealthextensions)

## Methods

### **WaitForHealthStatus(IBusControl, BusHealthStatus, TimeSpan)**

```csharp
public static Task<BusHealthStatus> WaitForHealthStatus(IBusControl busControl, BusHealthStatus expectedStatus, TimeSpan timeout)
```

#### Parameters

`busControl` [IBusControl](../masstransit/ibuscontrol)<br/>

`expectedStatus` [BusHealthStatus](../masstransit/bushealthstatus)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Task\<BusHealthStatus\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **WaitForHealthStatus(IEnumerable\<IBusControl\>, BusHealthStatus, TimeSpan)**

```csharp
public static Task<BusHealthStatus[]> WaitForHealthStatus(IEnumerable<IBusControl> busControls, BusHealthStatus expectedStatus, TimeSpan timeout)
```

#### Parameters

`busControls` [IEnumerable\<IBusControl\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`expectedStatus` [BusHealthStatus](../masstransit/bushealthstatus)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Task\<BusHealthStatus[]\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **WaitForHealthStatus(IBusControl, BusHealthStatus, CancellationToken)**

```csharp
public static Task<BusHealthStatus> WaitForHealthStatus(IBusControl busControl, BusHealthStatus expectedStatus, CancellationToken cancellationToken)
```

#### Parameters

`busControl` [IBusControl](../masstransit/ibuscontrol)<br/>

`expectedStatus` [BusHealthStatus](../masstransit/bushealthstatus)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<BusHealthStatus\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **WaitForHealthStatus(IEnumerable\<IBusControl\>, BusHealthStatus, CancellationToken)**

```csharp
public static Task<BusHealthStatus[]> WaitForHealthStatus(IEnumerable<IBusControl> busControls, BusHealthStatus expectedStatus, CancellationToken cancellationToken)
```

#### Parameters

`busControls` [IEnumerable\<IBusControl\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`expectedStatus` [BusHealthStatus](../masstransit/bushealthstatus)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<BusHealthStatus[]\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
