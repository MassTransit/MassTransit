---

title: EntitySettings

---

# EntitySettings

Namespace: MassTransit.SqlTransport

```csharp
public interface EntitySettings
```

## Properties

### **EntityName**

```csharp
public abstract string EntityName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **AutoDeleteOnIdle**

Idle time before queue should be deleted (consumer-idle, not producer)

```csharp
public abstract Nullable<TimeSpan> AutoDeleteOnIdle { get; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
