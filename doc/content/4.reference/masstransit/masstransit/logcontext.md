---

title: LogContext

---

# LogContext

Namespace: MassTransit

```csharp
public static class LogContext
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [LogContext](../masstransit/logcontext)

## Properties

### **Critical**

```csharp
public static Nullable<EnabledLogger> Critical { get; }
```

#### Property Value

[Nullable\<EnabledLogger\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Debug**

```csharp
public static Nullable<EnabledLogger> Debug { get; }
```

#### Property Value

[Nullable\<EnabledLogger\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Error**

```csharp
public static Nullable<EnabledLogger> Error { get; }
```

#### Property Value

[Nullable\<EnabledLogger\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Info**

```csharp
public static Nullable<EnabledLogger> Info { get; }
```

#### Property Value

[Nullable\<EnabledLogger\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Trace**

```csharp
public static Nullable<EnabledLogger> Trace { get; }
```

#### Property Value

[Nullable\<EnabledLogger\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Warning**

```csharp
public static Nullable<EnabledLogger> Warning { get; }
```

#### Property Value

[Nullable\<EnabledLogger\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Current**

Gets or sets the current operation (Activity) for the current thread. This flows
 across async calls.

```csharp
public static ILogContext Current { get; set; }
```

#### Property Value

[ILogContext](../masstransit-logging/ilogcontext)<br/>

## Methods

### **ConfigureCurrentLogContext(ILoggerFactory)**

```csharp
public static void ConfigureCurrentLogContext(ILoggerFactory loggerFactory)
```

#### Parameters

`loggerFactory` ILoggerFactory<br/>

### **ConfigureCurrentLogContext(ILogger)**

Configure the current [LogContext](../masstransit/logcontext) using the specified , which will be
 used for all log output.

```csharp
public static void ConfigureCurrentLogContext(ILogger logger)
```

#### Parameters

`logger` ILogger<br/>
An existing logger

### **CreateLogContext(String)**

```csharp
public static ILogContext CreateLogContext(string categoryName)
```

#### Parameters

`categoryName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ILogContext](../masstransit-logging/ilogcontext)<br/>

### **ConfigureCurrentLogContextIfNull(IServiceProvider)**

If [LogContext.Current](logcontext#current) is not null or the null logger, configure the current LogContext
 using the specified service provider.

```csharp
public static void ConfigureCurrentLogContextIfNull(IServiceProvider provider)
```

#### Parameters

`provider` IServiceProvider<br/>

### **SetCurrentIfNull(ILogContext)**

```csharp
public static void SetCurrentIfNull(ILogContext context)
```

#### Parameters

`context` [ILogContext](../masstransit-logging/ilogcontext)<br/>

### **Define\<T1\>(LogLevel, String)**

```csharp
public static LogMessage<T1> Define<T1>(LogLevel logLevel, string formatString)
```

#### Type Parameters

`T1`<br/>

#### Parameters

`logLevel` LogLevel<br/>

`formatString` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[LogMessage\<T1\>](../masstransit-logging/logmessage-1)<br/>

### **Define\<T1, T2\>(LogLevel, String)**

```csharp
public static LogMessage<T1, T2> Define<T1, T2>(LogLevel logLevel, string formatString)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

#### Parameters

`logLevel` LogLevel<br/>

`formatString` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[LogMessage\<T1, T2\>](../masstransit-logging/logmessage-2)<br/>

### **DefineMessage\<T1, T2\>(LogLevel, String)**

```csharp
public static LogMessage<T1, T2> DefineMessage<T1, T2>(LogLevel logLevel, string formatString)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

#### Parameters

`logLevel` LogLevel<br/>

`formatString` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[LogMessage\<T1, T2\>](../masstransit-logging/logmessage-2)<br/>

### **Define\<T1, T2, T3\>(LogLevel, String)**

```csharp
public static LogMessage<T1, T2, T3> Define<T1, T2, T3>(LogLevel logLevel, string formatString)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`T3`<br/>

#### Parameters

`logLevel` LogLevel<br/>

`formatString` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[LogMessage\<T1, T2, T3\>](../masstransit-logging/logmessage-3)<br/>

### **DefineMessage\<T1, T2, T3\>(LogLevel, String)**

```csharp
public static LogMessage<T1, T2, T3> DefineMessage<T1, T2, T3>(LogLevel logLevel, string formatString)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`T3`<br/>

#### Parameters

`logLevel` LogLevel<br/>

`formatString` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[LogMessage\<T1, T2, T3\>](../masstransit-logging/logmessage-3)<br/>

### **Define\<T1, T2, T3, T4\>(LogLevel, String)**

```csharp
public static LogMessage<T1, T2, T3, T4> Define<T1, T2, T3, T4>(LogLevel logLevel, string formatString)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`T3`<br/>

`T4`<br/>

#### Parameters

`logLevel` LogLevel<br/>

`formatString` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[LogMessage\<T1, T2, T3, T4\>](../masstransit-logging/logmessage-4)<br/>

### **DefineMessage\<T1, T2, T3, T4\>(LogLevel, String)**

```csharp
public static LogMessage<T1, T2, T3, T4> DefineMessage<T1, T2, T3, T4>(LogLevel logLevel, string formatString)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`T3`<br/>

`T4`<br/>

#### Parameters

`logLevel` LogLevel<br/>

`formatString` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[LogMessage\<T1, T2, T3, T4\>](../masstransit-logging/logmessage-4)<br/>

### **Define\<T1, T2, T3, T4, T5\>(LogLevel, String)**

```csharp
public static LogMessage<T1, T2, T3, T4, T5> Define<T1, T2, T3, T4, T5>(LogLevel logLevel, string formatString)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`T3`<br/>

`T4`<br/>

`T5`<br/>

#### Parameters

`logLevel` LogLevel<br/>

`formatString` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[LogMessage\<T1, T2, T3, T4, T5\>](../masstransit-logging/logmessage-5)<br/>

### **DefineMessage\<T1, T2, T3, T4, T5\>(LogLevel, String)**

```csharp
public static LogMessage<T1, T2, T3, T4, T5> DefineMessage<T1, T2, T3, T4, T5>(LogLevel logLevel, string formatString)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`T3`<br/>

`T4`<br/>

`T5`<br/>

#### Parameters

`logLevel` LogLevel<br/>

`formatString` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[LogMessage\<T1, T2, T3, T4, T5\>](../masstransit-logging/logmessage-5)<br/>
