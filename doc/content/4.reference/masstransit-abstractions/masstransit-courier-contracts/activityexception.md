---

title: ActivityException

---

# ActivityException

Namespace: MassTransit.Courier.Contracts

Capture the exception information thrown by an activity

```csharp
public interface ActivityException
```

## Properties

### **ExecutionId**

The tracking number of the activity that threw the exception

```csharp
public abstract Guid ExecutionId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Timestamp**

The point in time when the exception occurred

```csharp
public abstract DateTime Timestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Elapsed**

The time from when the routing slip was created until the exception occurred

```csharp
public abstract TimeSpan Elapsed { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **Name**

The name of the activity that caused the exception

```csharp
public abstract string Name { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Host**

The host where the exception was thrown

```csharp
public abstract HostInfo Host { get; }
```

#### Property Value

[HostInfo](../masstransit/hostinfo)<br/>

### **ExceptionInfo**

The exception details

```csharp
public abstract ExceptionInfo ExceptionInfo { get; }
```

#### Property Value

[ExceptionInfo](../masstransit/exceptioninfo)<br/>
