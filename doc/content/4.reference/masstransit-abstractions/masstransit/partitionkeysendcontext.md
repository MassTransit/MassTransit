---

title: PartitionKeySendContext

---

# PartitionKeySendContext

Namespace: MassTransit

```csharp
public interface PartitionKeySendContext
```

## Properties

### **PartitionKey**

The partition key for the message (defaults to "")

```csharp
public abstract string PartitionKey { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
