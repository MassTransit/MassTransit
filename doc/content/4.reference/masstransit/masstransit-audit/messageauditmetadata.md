---

title: MessageAuditMetadata

---

# MessageAuditMetadata

Namespace: MassTransit.Audit

```csharp
public class MessageAuditMetadata
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageAuditMetadata](../masstransit-audit/messageauditmetadata)

## Properties

### **MessageId**

```csharp
public Nullable<Guid> MessageId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConversationId**

```csharp
public Nullable<Guid> ConversationId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **CorrelationId**

```csharp
public Nullable<Guid> CorrelationId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **InitiatorId**

```csharp
public Nullable<Guid> InitiatorId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **RequestId**

```csharp
public Nullable<Guid> RequestId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SentTime**

```csharp
public Nullable<DateTime> SentTime { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SourceAddress**

```csharp
public string SourceAddress { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **DestinationAddress**

```csharp
public string DestinationAddress { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **InputAddress**

```csharp
public string InputAddress { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ResponseAddress**

```csharp
public string ResponseAddress { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **FaultAddress**

```csharp
public string FaultAddress { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ContextType**

```csharp
public string ContextType { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Headers**

```csharp
public Dictionary<string, string> Headers { get; set; }
```

#### Property Value

[Dictionary\<String, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **Custom**

```csharp
public Dictionary<string, string> Custom { get; set; }
```

#### Property Value

[Dictionary\<String, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

## Constructors

### **MessageAuditMetadata()**

```csharp
public MessageAuditMetadata()
```
