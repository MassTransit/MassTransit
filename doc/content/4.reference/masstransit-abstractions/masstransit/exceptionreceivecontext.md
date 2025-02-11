---

title: ExceptionReceiveContext

---

# ExceptionReceiveContext

Namespace: MassTransit

```csharp
public interface ExceptionReceiveContext : ReceiveContext, PipeContext
```

Implements [ReceiveContext](../masstransit/receivecontext), [PipeContext](../masstransit/pipecontext)

## Properties

### **Exception**

The exception that was thrown

```csharp
public abstract Exception Exception { get; }
```

#### Property Value

[Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **ExceptionTimestamp**

The time at which the exception was thrown

```csharp
public abstract DateTime ExceptionTimestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **ExceptionInfo**

The exception info, suitable for inclusion in a fault message

```csharp
public abstract ExceptionInfo ExceptionInfo { get; }
```

#### Property Value

[ExceptionInfo](../masstransit/exceptioninfo)<br/>

### **ExceptionHeaders**

Additional headers added to the transport message when moved to the error queue

```csharp
public abstract SendHeaders ExceptionHeaders { get; }
```

#### Property Value

[SendHeaders](../masstransit/sendheaders)<br/>
