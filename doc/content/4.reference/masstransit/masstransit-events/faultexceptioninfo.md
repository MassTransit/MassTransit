---

title: FaultExceptionInfo

---

# FaultExceptionInfo

Namespace: MassTransit.Events

```csharp
public class FaultExceptionInfo : ExceptionInfo
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FaultExceptionInfo](../masstransit-events/faultexceptioninfo)<br/>
Implements [ExceptionInfo](../../masstransit-abstractions/masstransit/exceptioninfo)

## Properties

### **ExceptionType**

```csharp
public string ExceptionType { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **InnerException**

```csharp
public ExceptionInfo InnerException { get; set; }
```

#### Property Value

[ExceptionInfo](../../masstransit-abstractions/masstransit/exceptioninfo)<br/>

### **StackTrace**

```csharp
public string StackTrace { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Message**

```csharp
public string Message { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Source**

```csharp
public string Source { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Data**

```csharp
public IDictionary<string, object> Data { get; set; }
```

#### Property Value

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

## Constructors

### **FaultExceptionInfo()**

```csharp
public FaultExceptionInfo()
```

### **FaultExceptionInfo(Exception)**

```csharp
public FaultExceptionInfo(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
