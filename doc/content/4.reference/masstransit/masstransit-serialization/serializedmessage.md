---

title: SerializedMessage

---

# SerializedMessage

Namespace: MassTransit.Serialization

The content of a serialized message

```csharp
public interface SerializedMessage
```

## Properties

### **Destination**

The destination for the serialized message

```csharp
public abstract Uri Destination { get; }
```

#### Property Value

Uri<br/>

### **ContentType**

The content type of the serializer used

```csharp
public abstract string ContentType { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ExpirationTime**

```csharp
public abstract string ExpirationTime { get; }
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

### **Body**

```csharp
public abstract string Body { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

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

### **HeadersAsJson**

```csharp
public abstract string HeadersAsJson { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **PayloadMessageHeadersAsJson**

```csharp
public abstract string PayloadMessageHeadersAsJson { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
