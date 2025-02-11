---

title: ValidationResultExtensions

---

# ValidationResultExtensions

Namespace: MassTransit

```csharp
public static class ValidationResultExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValidationResultExtensions](../masstransit/validationresultextensions)

## Methods

### **Failure(ISpecification, String)**

```csharp
public static ValidationResult Failure(ISpecification configurator, string message)
```

#### Parameters

`configurator` [ISpecification](../masstransit/ispecification)<br/>

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ValidationResult](../masstransit/validationresult)<br/>

### **Failure(ISpecification, String, String)**

```csharp
public static ValidationResult Failure(ISpecification configurator, string key, string message)
```

#### Parameters

`configurator` [ISpecification](../masstransit/ispecification)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ValidationResult](../masstransit/validationresult)<br/>

### **Failure(ISpecification, String, String, String)**

```csharp
public static ValidationResult Failure(ISpecification configurator, string key, string value, string message)
```

#### Parameters

`configurator` [ISpecification](../masstransit/ispecification)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ValidationResult](../masstransit/validationresult)<br/>

### **Warning(ISpecification, String)**

```csharp
public static ValidationResult Warning(ISpecification configurator, string message)
```

#### Parameters

`configurator` [ISpecification](../masstransit/ispecification)<br/>

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ValidationResult](../masstransit/validationresult)<br/>

### **Warning(ISpecification, String, String)**

```csharp
public static ValidationResult Warning(ISpecification configurator, string key, string message)
```

#### Parameters

`configurator` [ISpecification](../masstransit/ispecification)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ValidationResult](../masstransit/validationresult)<br/>

### **Warning(ISpecification, String, String, String)**

```csharp
public static ValidationResult Warning(ISpecification configurator, string key, string value, string message)
```

#### Parameters

`configurator` [ISpecification](../masstransit/ispecification)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ValidationResult](../masstransit/validationresult)<br/>

### **Success(ISpecification, String)**

```csharp
public static ValidationResult Success(ISpecification configurator, string message)
```

#### Parameters

`configurator` [ISpecification](../masstransit/ispecification)<br/>

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ValidationResult](../masstransit/validationresult)<br/>

### **Success(ISpecification, String, String)**

```csharp
public static ValidationResult Success(ISpecification configurator, string key, string message)
```

#### Parameters

`configurator` [ISpecification](../masstransit/ispecification)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ValidationResult](../masstransit/validationresult)<br/>

### **Success(ISpecification, String, String, String)**

```csharp
public static ValidationResult Success(ISpecification configurator, string key, string value, string message)
```

#### Parameters

`configurator` [ISpecification](../masstransit/ispecification)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ValidationResult](../masstransit/validationresult)<br/>

### **WithParentKey(ValidationResult, String)**

```csharp
public static ValidationResult WithParentKey(ValidationResult result, string parentKey)
```

#### Parameters

`result` [ValidationResult](../masstransit/validationresult)<br/>

`parentKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ValidationResult](../masstransit/validationresult)<br/>
