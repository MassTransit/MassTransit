---

title: RequestStarted

---

# RequestStarted

Namespace: MassTransit.Contracts

Published when a saga starts to process a request, but a subsequent operation (such as another request) is
 pending.

```csharp
public interface RequestStarted
```

## Properties

### **CorrelationId**

The saga correlationId, used to reconnect to the saga once the request is completed

```csharp
public abstract Guid CorrelationId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **RequestId**

The RequestId header value that was specified in the original request

```csharp
public abstract Guid RequestId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **ResponseAddress**

The ResponseAddress header value from the original request

```csharp
public abstract Uri ResponseAddress { get; }
```

#### Property Value

Uri<br/>

### **FaultAddress**

The FaultAddress header value from the original request

```csharp
public abstract Uri FaultAddress { get; }
```

#### Property Value

Uri<br/>

### **ExpirationTime**

The expiration time for this request, which if completed after, the response is discarded

```csharp
public abstract Nullable<DateTime> ExpirationTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

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
