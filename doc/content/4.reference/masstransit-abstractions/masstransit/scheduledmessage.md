---

title: ScheduledMessage

---

# ScheduledMessage

Namespace: MassTransit

```csharp
public interface ScheduledMessage
```

## Properties

### **TokenId**

```csharp
public abstract Guid TokenId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **ScheduledTime**

```csharp
public abstract DateTime ScheduledTime { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Destination**

```csharp
public abstract Uri Destination { get; }
```

#### Property Value

Uri<br/>
