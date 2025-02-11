---

title: PartitionKeyConsumeContext

---

# PartitionKeyConsumeContext

Namespace: MassTransit

```csharp
public interface PartitionKeyConsumeContext
```

## Properties

### **PartitionKey**

The partition key for the message (defaults to "")

```csharp
public abstract string PartitionKey { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
