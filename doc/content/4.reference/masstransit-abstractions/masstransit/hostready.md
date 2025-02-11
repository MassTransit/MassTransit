---

title: HostReady

---

# HostReady

Namespace: MassTransit

```csharp
public interface HostReady
```

## Properties

### **HostAddress**

The Host address

```csharp
public abstract Uri HostAddress { get; }
```

#### Property Value

Uri<br/>

### **ReceiveEndpoints**

The receive endpoints that were started on the host

```csharp
public abstract ReceiveEndpointReady[] ReceiveEndpoints { get; }
```

#### Property Value

[ReceiveEndpointReady[]](../masstransit/receiveendpointready)<br/>

### **Riders**

The riders that were started on the host

```csharp
public abstract RiderReady[] Riders { get; }
```

#### Property Value

[RiderReady[]](../masstransit/riderready)<br/>
