---

title: SetConcurrencyLimit

---

# SetConcurrencyLimit

Namespace: MassTransit.Contracts

Sets the concurrency limit of a concurrency limit filter

```csharp
public interface SetConcurrencyLimit
```

## Properties

### **Timestamp**

The timestamp at which the adjustment command was sent

```csharp
public abstract Nullable<DateTime> Timestamp { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Id**

The identifier of the concurrency limit to set (optional)

```csharp
public abstract string Id { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ConcurrencyLimit**

The new concurrency limit for the filter

```csharp
public abstract int ConcurrencyLimit { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
