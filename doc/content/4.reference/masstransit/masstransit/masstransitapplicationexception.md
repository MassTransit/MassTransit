---

title: MassTransitApplicationException

---

# MassTransitApplicationException

Namespace: MassTransit

For use by application developers to include additional data elements along with the exception, which will be
 transferred to the  of the  event.

```csharp
public class MassTransitApplicationException : Exception, ISerializable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception) → [MassTransitApplicationException](../masstransit/masstransitapplicationexception)<br/>
Implements [ISerializable](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.iserializable)

## Properties

### **Data**

```csharp
public IDictionary Data { get; }
```

#### Property Value

[IDictionary](https://learn.microsoft.com/en-us/dotnet/api/system.collections.idictionary)<br/>

### **ApplicationData**

```csharp
public IDictionary<string, object> ApplicationData { get; }
```

#### Property Value

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

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

### **MassTransitApplicationException(Exception)**

```csharp
public MassTransitApplicationException(Exception innerException)
```

#### Parameters

`innerException` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **MassTransitApplicationException(Exception, Object)**

```csharp
public MassTransitApplicationException(Exception innerException, object values)
```

#### Parameters

`innerException` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

### **MassTransitApplicationException(Exception, IEnumerable\<KeyValuePair\<String, Object\>\>)**

```csharp
public MassTransitApplicationException(Exception innerException, IEnumerable<KeyValuePair<string, object>> values)
```

#### Parameters

`innerException` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`values` [IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **MassTransitApplicationException(String, Exception)**

```csharp
public MassTransitApplicationException(string message, Exception innerException)
```

#### Parameters

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`innerException` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **MassTransitApplicationException(String, Exception, Object)**

```csharp
public MassTransitApplicationException(string message, Exception innerException, object values)
```

#### Parameters

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`innerException` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

### **MassTransitApplicationException(String, Exception, IEnumerable\<KeyValuePair\<String, Object\>\>)**

```csharp
public MassTransitApplicationException(string message, Exception innerException, IEnumerable<KeyValuePair<string, object>> values)
```

#### Parameters

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`innerException` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`values` [IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
