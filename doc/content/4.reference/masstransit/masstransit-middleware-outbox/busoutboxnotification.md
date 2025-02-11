---

title: BusOutboxNotification

---

# BusOutboxNotification

Namespace: MassTransit.Middleware.Outbox

```csharp
public class BusOutboxNotification : IBusOutboxNotification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BusOutboxNotification](../masstransit-middleware-outbox/busoutboxnotification)<br/>
Implements [IBusOutboxNotification](../masstransit-middleware-outbox/ibusoutboxnotification)

## Constructors

### **BusOutboxNotification(IOptions\<OutboxDeliveryServiceOptions\>)**

```csharp
public BusOutboxNotification(IOptions<OutboxDeliveryServiceOptions> options)
```

#### Parameters

`options` IOptions\<OutboxDeliveryServiceOptions\><br/>

## Methods

### **WaitForDelivery(CancellationToken)**

```csharp
public Task WaitForDelivery(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Delivered()**

```csharp
public void Delivered()
```
