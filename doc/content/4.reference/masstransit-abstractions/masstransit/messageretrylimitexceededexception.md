---

title: MessageRetryLimitExceededException

---

# MessageRetryLimitExceededException

Namespace: MassTransit

```csharp
public class MessageRetryLimitExceededException : TransportException, ISerializable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception) → [MassTransitException](../masstransit/masstransitexception) → [AbstractUriException](../masstransit/abstracturiexception) → [TransportException](../masstransit/transportexception) → [MessageRetryLimitExceededException](../masstransit/messageretrylimitexceededexception)<br/>
Implements [ISerializable](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.iserializable)

## Properties

### **Uri**

```csharp
public Uri Uri { get; protected set; }
```

#### Property Value

Uri<br/>

### **TargetSite**

```csharp
public MethodBase TargetSite { get; }
```

#### Property Value

[MethodBase](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.methodbase)<br/>

### **Message**

```csharp
public string Message { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Data**

```csharp
public IDictionary Data { get; }
```

#### Property Value

[IDictionary](https://learn.microsoft.com/en-us/dotnet/api/system.collections.idictionary)<br/>

### **InnerException**

```csharp
public Exception InnerException { get; }
```

#### Property Value

[Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **HelpLink**

```csharp
public string HelpLink { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Source**

```csharp
public string Source { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **HResult**

```csharp
public int HResult { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **StackTrace**

```csharp
public string StackTrace { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **MessageRetryLimitExceededException()**

```csharp
public MessageRetryLimitExceededException()
```

### **MessageRetryLimitExceededException(Uri)**

```csharp
public MessageRetryLimitExceededException(Uri uri)
```

#### Parameters

`uri` Uri<br/>

### **MessageRetryLimitExceededException(Uri, String)**

```csharp
public MessageRetryLimitExceededException(Uri uri, string message)
```

#### Parameters

`uri` Uri<br/>

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **MessageRetryLimitExceededException(Uri, String, Exception)**

```csharp
public MessageRetryLimitExceededException(Uri uri, string message, Exception innerException)
```

#### Parameters

`uri` Uri<br/>

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`innerException` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
