---

title: RoutingSlipCompensationFailed

---

# RoutingSlipCompensationFailed

Namespace: MassTransit.Courier.Contracts

```csharp
public interface RoutingSlipCompensationFailed
```

## Properties

### **TrackingNumber**

The tracking number of the routing slip that faulted

```csharp
public abstract Guid TrackingNumber { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Timestamp**

The date/time when the routing slip compensation was finished

```csharp
public abstract DateTime Timestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Duration**

The duration of the activity execution

```csharp
public abstract TimeSpan Duration { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **Host**

The host that executed the activity

```csharp
public abstract HostInfo Host { get; }
```

#### Property Value

[HostInfo](../masstransit/hostinfo)<br/>

### **ExceptionInfo**

The exception information from the faulting activity

```csharp
public abstract ExceptionInfo ExceptionInfo { get; }
```

#### Property Value

[ExceptionInfo](../masstransit/exceptioninfo)<br/>

### **Variables**

The variables that were present once the routing slip completed, can be used
 to capture the output of the slip - real events should likely be used for real
 completion items but this is useful for some cases

```csharp
public abstract IDictionary<string, object> Variables { get; }
```

#### Property Value

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>
