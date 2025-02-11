---

title: ValidationResult

---

# ValidationResult

Namespace: MassTransit

Reports information about the configuration before configuring
 so that corrections can be made without allocating resources, etc.

```csharp
public interface ValidationResult
```

## Properties

### **Disposition**

The disposition of the result, any Failure items will prevent
 the configuration from completing.

```csharp
public abstract ValidationResultDisposition Disposition { get; }
```

#### Property Value

[ValidationResultDisposition](../masstransit/validationresultdisposition)<br/>

### **Message**

The message associated with the result

```csharp
public abstract string Message { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Key**

The key associated with the result (chained if configurators are nested)

```csharp
public abstract string Key { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Value**

The value associated with the result

```csharp
public abstract string Value { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
