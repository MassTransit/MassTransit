---

title: BusDepotExtensions

---

# BusDepotExtensions

Namespace: MassTransit

```csharp
public static class BusDepotExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BusDepotExtensions](../masstransit/busdepotextensions)

## Methods

### **Start(IBusDepot, TimeSpan, CancellationToken)**

```csharp
public static Task Start(IBusDepot depot, TimeSpan timeout, CancellationToken cancellationToken)
```

#### Parameters

`depot` [IBusDepot](../masstransit/ibusdepot)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Stop(IBusDepot, TimeSpan, CancellationToken)**

```csharp
public static Task Stop(IBusDepot depot, TimeSpan timeout, CancellationToken cancellationToken)
```

#### Parameters

`depot` [IBusDepot](../masstransit/ibusdepot)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
