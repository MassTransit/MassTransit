---

title: MassTransitHostedService

---

# MassTransitHostedService

Namespace: MassTransit

```csharp
public class MassTransitHostedService : IHostedService, IAsyncDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MassTransitHostedService](../masstransit/masstransithostedservice)<br/>
Implements IHostedService, [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Constructors

### **MassTransitHostedService(IBusDepot, IOptions\<MassTransitHostOptions\>)**

```csharp
public MassTransitHostedService(IBusDepot depot, IOptions<MassTransitHostOptions> options)
```

#### Parameters

`depot` [IBusDepot](../../masstransit-abstractions/masstransit/ibusdepot)<br/>

`options` IOptions\<MassTransitHostOptions\><br/>

## Methods

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>

### **StartAsync(CancellationToken)**

```csharp
public Task StartAsync(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **StopAsync(CancellationToken)**

```csharp
public Task StopAsync(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
