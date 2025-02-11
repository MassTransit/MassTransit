---

title: BusDepot

---

# BusDepot

Namespace: MassTransit.Transports

```csharp
public class BusDepot : IBusDepot
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BusDepot](../masstransit-transports/busdepot)<br/>
Implements [IBusDepot](../../masstransit-abstractions/masstransit/ibusdepot)

## Constructors

### **BusDepot(IEnumerable\<IBusInstance\>, ILogger\<BusDepot\>)**

```csharp
public BusDepot(IEnumerable<IBusInstance> instances, ILogger<BusDepot> logger)
```

#### Parameters

`instances` [IEnumerable\<IBusInstance\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`logger` ILogger\<BusDepot\><br/>

## Methods

### **Start(CancellationToken)**

```csharp
public Task Start(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Stop(CancellationToken)**

```csharp
public Task Stop(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
