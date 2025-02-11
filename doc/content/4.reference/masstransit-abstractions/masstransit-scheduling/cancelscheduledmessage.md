---

title: CancelScheduledMessage

---

# CancelScheduledMessage

Namespace: MassTransit.Scheduling

```csharp
public interface CancelScheduledMessage
```

## Properties

### **CorrelationId**

The cancel scheduled message correlationId

```csharp
public abstract Guid CorrelationId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Timestamp**

The date/time this message was created

```csharp
public abstract DateTime Timestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **TokenId**

The token of the scheduled message

```csharp
public abstract Guid TokenId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
