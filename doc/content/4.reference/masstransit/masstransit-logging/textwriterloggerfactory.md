---

title: TextWriterLoggerFactory

---

# TextWriterLoggerFactory

Namespace: MassTransit.Logging

```csharp
public class TextWriterLoggerFactory : ILoggerFactory, IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TextWriterLoggerFactory](../masstransit-logging/textwriterloggerfactory)<br/>
Implements ILoggerFactory, [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **Writer**

```csharp
public TextWriter Writer { get; }
```

#### Property Value

[TextWriter](https://learn.microsoft.com/en-us/dotnet/api/system.io.textwriter)<br/>

## Constructors

### **TextWriterLoggerFactory(TextWriter, IOptions\<TextWriterLoggerOptions\>)**

```csharp
public TextWriterLoggerFactory(TextWriter textWriter, IOptions<TextWriterLoggerOptions> options)
```

#### Parameters

`textWriter` [TextWriter](https://learn.microsoft.com/en-us/dotnet/api/system.io.textwriter)<br/>

`options` IOptions\<TextWriterLoggerOptions\><br/>

## Methods

### **CreateLogger(String)**

```csharp
public ILogger CreateLogger(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

ILogger<br/>

### **AddProvider(ILoggerProvider)**

```csharp
public void AddProvider(ILoggerProvider provider)
```

#### Parameters

`provider` ILoggerProvider<br/>

### **Dispose()**

```csharp
public void Dispose()
```
