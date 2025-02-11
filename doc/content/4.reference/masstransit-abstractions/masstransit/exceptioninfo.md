---

title: ExceptionInfo

---

# ExceptionInfo

Namespace: MassTransit

An exception information that is serializable

```csharp
public interface ExceptionInfo
```

## Properties

### **ExceptionType**

The type name of the exception

```csharp
public abstract string ExceptionType { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **InnerException**

The inner exception if present (also converted to ExceptionInfo)

```csharp
public abstract ExceptionInfo InnerException { get; }
```

#### Property Value

[ExceptionInfo](../masstransit/exceptioninfo)<br/>

### **StackTrace**

The stack trace of the exception site

```csharp
public abstract string StackTrace { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Message**

The exception message

```csharp
public abstract string Message { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Source**

The exception source

```csharp
public abstract string Source { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Data**

```csharp
public abstract IDictionary<string, object> Data { get; }
```

#### Property Value

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>
