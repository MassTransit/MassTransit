---

title: JsonMessageEnvelope

---

# JsonMessageEnvelope

Namespace: MassTransit.Serialization

```csharp
public class JsonMessageEnvelope : MessageEnvelope
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JsonMessageEnvelope](../masstransit-serialization/jsonmessageenvelope)<br/>
Implements [MessageEnvelope](../../masstransit-abstractions/masstransit-serialization/messageenvelope)

## Properties

### **MessageId**

```csharp
public string MessageId { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **RequestId**

```csharp
public string RequestId { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **CorrelationId**

```csharp
public string CorrelationId { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ConversationId**

```csharp
public string ConversationId { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **InitiatorId**

```csharp
public string InitiatorId { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

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

### **MessageType**

```csharp
public String[] MessageType { get; set; }
```

#### Property Value

[String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Message**

```csharp
public object Message { get; set; }
```

#### Property Value

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

### **ExpirationTime**

```csharp
public Nullable<DateTime> ExpirationTime { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SentTime**

```csharp
public Nullable<DateTime> SentTime { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Headers**

```csharp
public Dictionary<string, object> Headers { get; set; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **Host**

```csharp
public HostInfo Host { get; set; }
```

#### Property Value

[HostInfo](../../masstransit-abstractions/masstransit/hostinfo)<br/>

## Constructors

### **JsonMessageEnvelope()**

```csharp
public JsonMessageEnvelope()
```

### **JsonMessageEnvelope(SendContext, Object)**

```csharp
public JsonMessageEnvelope(SendContext context, object message)
```

#### Parameters

`context` [SendContext](../../masstransit-abstractions/masstransit/sendcontext)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

### **JsonMessageEnvelope(MessageContext, Object, String[])**

```csharp
public JsonMessageEnvelope(MessageContext context, object message, String[] messageTypeNames)
```

#### Parameters

`context` [MessageContext](../../masstransit-abstractions/masstransit/messagecontext)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageTypeNames` [String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **JsonMessageEnvelope(MessageEnvelope)**

```csharp
public JsonMessageEnvelope(MessageEnvelope envelope)
```

#### Parameters

`envelope` [MessageEnvelope](../../masstransit-abstractions/masstransit-serialization/messageenvelope)<br/>

## Methods

### **Update(SendContext)**

```csharp
public void Update(SendContext context)
```

#### Parameters

`context` [SendContext](../../masstransit-abstractions/masstransit/sendcontext)<br/>
