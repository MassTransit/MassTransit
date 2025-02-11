---

title: RequestFaulted

---

# RequestFaulted

Namespace: MassTransit.Contracts

```csharp
public interface RequestFaulted
```

## Properties

### **CorrelationId**

The saga correlationId, used to reconnect to the saga once the request is completed

```csharp
public abstract Guid CorrelationId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **PayloadType**

The payload types supported by the payload

```csharp
public abstract String[] PayloadType { get; }
```

#### Property Value

[String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Payload**

The actual message payload

```csharp
public abstract object Payload { get; }
```

#### Property Value

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
