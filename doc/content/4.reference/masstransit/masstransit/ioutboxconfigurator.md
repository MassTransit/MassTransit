---

title: IOutboxConfigurator

---

# IOutboxConfigurator

Namespace: MassTransit

```csharp
public interface IOutboxConfigurator
```

## Properties

### **ConcurrentMessageDelivery**

Set to true if messages can be delivered to the broker concurrently. Concurrent delivery is faster, but does not match the order of the
 original publish/respond/send calls. Defaults to false to match existing behavior.

```csharp
public abstract bool ConcurrentMessageDelivery { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
