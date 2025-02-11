---

title: ReceiveEndpointReady

---

# ReceiveEndpointReady

Namespace: MassTransit

```csharp
public interface ReceiveEndpointReady : ReceiveEndpointEvent
```

Implements [ReceiveEndpointEvent](../masstransit/receiveendpointevent)

## Properties

### **IsStarted**

If true, the receive endpoint is actually ready, versus "fake-ready" for endpoints which do not auto-start

```csharp
public abstract bool IsStarted { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
