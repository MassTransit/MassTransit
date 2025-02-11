---

title: ReceiveTransportReady

---

# ReceiveTransportReady

Namespace: MassTransit

```csharp
public interface ReceiveTransportReady : ReceiveTransportEvent
```

Implements [ReceiveTransportEvent](../masstransit/receivetransportevent)

## Properties

### **IsStarted**

If true, the receive transport is actually ready, versus "fake-ready" for endpoints which do not auto-start

```csharp
public abstract bool IsStarted { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
