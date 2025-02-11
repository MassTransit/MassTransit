---

title: IBusOutboxNotification

---

# IBusOutboxNotification

Namespace: MassTransit.Middleware.Outbox

```csharp
public interface IBusOutboxNotification
```

## Methods

### **WaitForDelivery(CancellationToken)**

```csharp
Task WaitForDelivery(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Delivered()**

```csharp
void Delivered()
```
