---

title: TextWriterLogger

---

# TextWriterLogger

Namespace: MassTransit.Logging

```csharp
public class TextWriterLogger : ILogger
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TextWriterLogger](../masstransit-logging/textwriterlogger)<br/>
Implements ILogger

## Constructors

### **TextWriterLogger(TextWriterLoggerFactory, LogLevel)**

```csharp
public TextWriterLogger(TextWriterLoggerFactory factory, LogLevel logLevel)
```

#### Parameters

`factory` [TextWriterLoggerFactory](../masstransit-logging/textwriterloggerfactory)<br/>

`logLevel` LogLevel<br/>

## Methods

### **BeginScope\<TState\>(TState)**

```csharp
public IDisposable BeginScope<TState>(TState state)
```

#### Type Parameters

`TState`<br/>

#### Parameters

`state` TState<br/>

#### Returns

[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>

### **Log\<TState\>(LogLevel, EventId, TState, Exception, Func\<TState, Exception, String\>)**

```csharp
public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
```

#### Type Parameters

`TState`<br/>

#### Parameters

`logLevel` LogLevel<br/>

`eventId` EventId<br/>

`state` TState<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`formatter` [Func\<TState, Exception, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

### **IsEnabled(LogLevel)**

```csharp
public bool IsEnabled(LogLevel logLevel)
```

#### Parameters

`logLevel` LogLevel<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
