---

title: RequestCompleted

---

# RequestCompleted

Namespace: MassTransit.Contracts

Published by the saga when a request is completed, so that waiting requests can be completed, or redelivered to the
 saga for completion.

```csharp
public interface RequestCompleted
```

## Properties

### **CorrelationId**

The saga correlationId

```csharp
public abstract Guid CorrelationId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Timestamp**

The timestamp when the request was completed

```csharp
public abstract DateTime Timestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

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
