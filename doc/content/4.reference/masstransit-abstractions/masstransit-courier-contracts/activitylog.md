---

title: ActivityLog

---

# ActivityLog

Namespace: MassTransit.Courier.Contracts

Message contract for storing activity log data

```csharp
public interface ActivityLog
```

## Properties

### **ExecutionId**

The tracking number for completion of the activity

```csharp
public abstract Guid ExecutionId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Name**

The name of the activity that was completed

```csharp
public abstract string Name { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Timestamp**

The timestamp when the activity started

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
