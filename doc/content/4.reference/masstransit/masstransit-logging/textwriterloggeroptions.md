---

title: TextWriterLoggerOptions

---

# TextWriterLoggerOptions

Namespace: MassTransit.Logging

```csharp
public class TextWriterLoggerOptions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TextWriterLoggerOptions](../masstransit-logging/textwriterloggeroptions)

## Properties

### **LogLevel**

```csharp
public LogLevel LogLevel { get; set; }
```

#### Property Value

LogLevel<br/>

## Constructors

### **TextWriterLoggerOptions()**

```csharp
public TextWriterLoggerOptions()
```

## Methods

### **Disable(String)**

```csharp
public TextWriterLoggerOptions Disable(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[TextWriterLoggerOptions](../masstransit-logging/textwriterloggeroptions)<br/>

### **IsEnabled(String)**

```csharp
public bool IsEnabled(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
