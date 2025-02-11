---

title: AsyncBusHandle

---

# AsyncBusHandle

Namespace: MassTransit

```csharp
public class AsyncBusHandle : IAsyncBusHandle, IAsyncDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncBusHandle](../masstransit/asyncbushandle)<br/>
Implements [IAsyncBusHandle](../masstransit/iasyncbushandle), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Constructors

### **AsyncBusHandle(IBusDepot, ILogger\<MassTransitBus\>, IOptions\<MassTransitHostOptions\>)**

```csharp
public AsyncBusHandle(IBusDepot depot, ILogger<MassTransitBus> logger, IOptions<MassTransitHostOptions> options)
```

#### Parameters

`depot` [IBusDepot](../../masstransit-abstractions/masstransit/ibusdepot)<br/>

`logger` ILogger\<MassTransitBus\><br/>

`options` IOptions\<MassTransitHostOptions\><br/>

## Methods

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>
