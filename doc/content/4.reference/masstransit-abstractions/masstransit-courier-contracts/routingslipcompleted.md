---

title: RoutingSlipCompleted

---

# RoutingSlipCompleted

Namespace: MassTransit.Courier.Contracts

Published when a routing slip completes

```csharp
public interface RoutingSlipCompleted
```

## Properties

### **TrackingNumber**

The tracking number of the routing slip that completed

```csharp
public abstract Guid TrackingNumber { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Timestamp**

The date/time when the routing slip completed

```csharp
public abstract DateTime Timestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Duration**

The time from when the routing slip was created until the completion

```csharp
public abstract TimeSpan Duration { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **Variables**

The variables that were present once the routing slip completed, can be used
 to capture the output of the slip - real events should likely be used for real
 completion items but this is useful for some cases

```csharp
public abstract IDictionary<string, object> Variables { get; }
```

#### Property Value

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>
