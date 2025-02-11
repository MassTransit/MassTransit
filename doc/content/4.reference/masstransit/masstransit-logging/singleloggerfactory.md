---

title: SingleLoggerFactory

---

# SingleLoggerFactory

Namespace: MassTransit.Logging

```csharp
public class SingleLoggerFactory : ILoggerFactory, IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SingleLoggerFactory](../masstransit-logging/singleloggerfactory)<br/>
Implements ILoggerFactory, [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Constructors

### **SingleLoggerFactory(ILogger)**

```csharp
public SingleLoggerFactory(ILogger logger)
```

#### Parameters

`logger` ILogger<br/>

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
