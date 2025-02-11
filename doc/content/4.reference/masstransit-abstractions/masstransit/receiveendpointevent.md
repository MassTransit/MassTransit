---

title: ReceiveEndpointEvent

---

# ReceiveEndpointEvent

Namespace: MassTransit

```csharp
public interface ReceiveEndpointEvent
```

## Properties

### **InputAddress**

The input address of the receive endpoint

```csharp
public abstract Uri InputAddress { get; }
```

#### Property Value

Uri<br/>

### **ReceiveEndpoint**

The receive endpoint upon which the event occurred

```csharp
public abstract IReceiveEndpoint ReceiveEndpoint { get; }
```

#### Property Value

[IReceiveEndpoint](../masstransit/ireceiveendpoint)<br/>
