---

title: ILogContext

---

# ILogContext

Namespace: MassTransit.Logging

Used to provide access to logging and diagnostic services

```csharp
public interface ILogContext
```

## Properties

### **Logger**

```csharp
public abstract ILogger Logger { get; }
```

#### Property Value

ILogger<br/>

### **Messages**

The log context for all message movement, sent, received, etc.

```csharp
public abstract ILogContext Messages { get; }
```

#### Property Value

[ILogContext](../masstransit-logging/ilogcontext)<br/>

### **Critical**

```csharp
public abstract Nullable<EnabledLogger> Critical { get; }
```

#### Property Value

[Nullable\<EnabledLogger\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Debug**

```csharp
public abstract Nullable<EnabledLogger> Debug { get; }
```

#### Property Value

[Nullable\<EnabledLogger\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Error**

```csharp
public abstract Nullable<EnabledLogger> Error { get; }
```

#### Property Value

[Nullable\<EnabledLogger\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Info**

```csharp
public abstract Nullable<EnabledLogger> Info { get; }
```

#### Property Value

[Nullable\<EnabledLogger\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Trace**

```csharp
public abstract Nullable<EnabledLogger> Trace { get; }
```

#### Property Value

[Nullable\<EnabledLogger\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Warning**

```csharp
public abstract Nullable<EnabledLogger> Warning { get; }
```

#### Property Value

[Nullable\<EnabledLogger\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Methods

### **CreateLogContext(String)**

Creates a new  instance.

```csharp
ILogContext CreateLogContext(string categoryName)
```

#### Parameters

`categoryName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The category name for messages produced by the logger.

#### Returns

[ILogContext](../masstransit-logging/ilogcontext)<br/>
The .
