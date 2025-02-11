---

title: ActivityExecutionFaultedException

---

# ActivityExecutionFaultedException

Namespace: MassTransit

```csharp
public class ActivityExecutionFaultedException : ActivityExecutionException, ISerializable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception) → [MassTransitException](../masstransit/masstransitexception) → [CourierException](../masstransit/courierexception) → [ActivityExecutionException](../masstransit/activityexecutionexception) → [ActivityExecutionFaultedException](../masstransit/activityexecutionfaultedexception)<br/>
Implements [ISerializable](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.iserializable)

## Properties

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

### **ActivityExecutionFaultedException()**

```csharp
public ActivityExecutionFaultedException()
```

### **ActivityExecutionFaultedException(String)**

```csharp
public ActivityExecutionFaultedException(string message)
```

#### Parameters

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ActivityExecutionFaultedException(String, Exception)**

```csharp
public ActivityExecutionFaultedException(string message, Exception innerException)
```

#### Parameters

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`innerException` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
