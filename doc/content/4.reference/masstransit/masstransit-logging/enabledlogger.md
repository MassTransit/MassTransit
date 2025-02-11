---

title: EnabledLogger

---

# EnabledLogger

Namespace: MassTransit.Logging

```csharp
public struct EnabledLogger
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [EnabledLogger](../masstransit-logging/enabledlogger)

## Constructors

### **EnabledLogger(ILogger, LogLevel)**

```csharp
public EnabledLogger(ILogger logger, LogLevel level)
```

#### Parameters

`logger` ILogger<br/>

`level` LogLevel<br/>

## Methods

### **Log(String, Object[])**

```csharp
public void Log(string message, Object[] args)
```

#### Parameters

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`args` [Object[]](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

### **Log(Exception, String, Object[])**

```csharp
public void Log(Exception exception, string message, Object[] args)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`args` [Object[]](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
