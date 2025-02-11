---

title: SqlEndpointAddressException

---

# SqlEndpointAddressException

Namespace: MassTransit

```csharp
public sealed class SqlEndpointAddressException : AbstractUriException, ISerializable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception) → [MassTransitException](../../masstransit-abstractions/masstransit/masstransitexception) → [AbstractUriException](../../masstransit-abstractions/masstransit/abstracturiexception) → [SqlEndpointAddressException](../masstransit/sqlendpointaddressexception)<br/>
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

### **SqlEndpointAddressException()**

```csharp
public SqlEndpointAddressException()
```

### **SqlEndpointAddressException(Uri, String)**

```csharp
public SqlEndpointAddressException(Uri address, string message)
```

#### Parameters

`address` Uri<br/>

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **SqlEndpointAddressException(Uri, String, Exception)**

```csharp
public SqlEndpointAddressException(Uri address, string message, Exception innerException)
```

#### Parameters

`address` Uri<br/>

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`innerException` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **SqlEndpointAddressException(SerializationInfo, StreamingContext)**

#### Caution

Formatter-based serialization is obsolete and should not be used.

---

```csharp
public SqlEndpointAddressException(SerializationInfo info, StreamingContext context)
```

#### Parameters

`info` [SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo)<br/>

`context` [StreamingContext](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.streamingcontext)<br/>
