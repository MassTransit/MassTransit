---

title: BusLogContext

---

# BusLogContext

Namespace: MassTransit.Logging

```csharp
public class BusLogContext : ILogContext
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BusLogContext](../masstransit-logging/buslogcontext)<br/>
Implements [ILogContext](../masstransit-logging/ilogcontext)

## Properties

### **Logger**

```csharp
public ILogger Logger { get; }
```

#### Property Value

ILogger<br/>

### **Critical**

```csharp
public Nullable<EnabledLogger> Critical { get; }
```

#### Property Value

[Nullable\<EnabledLogger\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Debug**

```csharp
public Nullable<EnabledLogger> Debug { get; }
```

#### Property Value

[Nullable\<EnabledLogger\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Error**

```csharp
public Nullable<EnabledLogger> Error { get; }
```

#### Property Value

[Nullable\<EnabledLogger\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Info**

```csharp
public Nullable<EnabledLogger> Info { get; }
```

#### Property Value

[Nullable\<EnabledLogger\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Trace**

```csharp
public Nullable<EnabledLogger> Trace { get; }
```

#### Property Value

[Nullable\<EnabledLogger\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Warning**

```csharp
public Nullable<EnabledLogger> Warning { get; }
```

#### Property Value

[Nullable\<EnabledLogger\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **BusLogContext(ILoggerFactory)**

```csharp
public BusLogContext(ILoggerFactory loggerFactory)
```

#### Parameters

`loggerFactory` ILoggerFactory<br/>

## Methods

### **CreateLogContext(String)**

```csharp
public ILogContext CreateLogContext(string categoryName)
```

#### Parameters

`categoryName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ILogContext](../masstransit-logging/ilogcontext)<br/>
