---

title: RequestTimeoutExpired<TRequest>

---

# RequestTimeoutExpired\<TRequest\>

Namespace: MassTransit.Contracts

```csharp
public interface RequestTimeoutExpired<TRequest>
```

#### Type Parameters

`TRequest`<br/>

## Properties

### **CorrelationId**

The correlationId of the state machine

```csharp
public abstract Guid CorrelationId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Timestamp**

When the request expired

```csharp
public abstract DateTime Timestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **ExpirationTime**

The expiration time that was scheduled for the request

```csharp
public abstract DateTime ExpirationTime { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **RequestId**

The requestId of the request

```csharp
public abstract Guid RequestId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Message**

The original request message.

```csharp
public abstract TRequest Message { get; }
```

#### Property Value

TRequest<br/>
