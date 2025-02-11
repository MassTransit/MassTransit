---

title: MessageEnvelope

---

# MessageEnvelope

Namespace: MassTransit.Serialization

```csharp
public interface MessageEnvelope
```

## Properties

### **MessageId**

```csharp
public abstract string MessageId { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **RequestId**

```csharp
public abstract string RequestId { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **CorrelationId**

```csharp
public abstract string CorrelationId { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ConversationId**

```csharp
public abstract string ConversationId { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **InitiatorId**

```csharp
public abstract string InitiatorId { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **SourceAddress**

```csharp
public abstract string SourceAddress { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **DestinationAddress**

```csharp
public abstract string DestinationAddress { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ResponseAddress**

```csharp
public abstract string ResponseAddress { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **FaultAddress**

```csharp
public abstract string FaultAddress { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **MessageType**

```csharp
public abstract String[] MessageType { get; }
```

#### Property Value

[String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Message**

```csharp
public abstract object Message { get; }
```

#### Property Value

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

### **ExpirationTime**

```csharp
public abstract Nullable<DateTime> ExpirationTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SentTime**

```csharp
public abstract Nullable<DateTime> SentTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Headers**

```csharp
public abstract Dictionary<string, object> Headers { get; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **Host**

```csharp
public abstract HostInfo Host { get; }
```

#### Property Value

[HostInfo](../masstransit/hostinfo)<br/>
