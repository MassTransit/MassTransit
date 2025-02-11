---

title: ConcurrencyLimitUpdated

---

# ConcurrencyLimitUpdated

Namespace: MassTransit.Contracts

Published when the concurrency limit of a filter is updated.

```csharp
public interface ConcurrencyLimitUpdated
```

## Properties

### **Timestamp**

The actual time at which the adjustment was applied

```csharp
public abstract DateTime Timestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Id**

The identifier that was adjusted

```csharp
public abstract string Id { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ConcurrencyLimit**

The current concurrency limit value

```csharp
public abstract int ConcurrencyLimit { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
