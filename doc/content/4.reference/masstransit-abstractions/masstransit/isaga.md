---

title: ISaga

---

# ISaga

Namespace: MassTransit

Interface that specifies a class is usable as a saga instance, including
 the ability to get and set the CorrelationId on the saga instance.

```csharp
public interface ISaga
```

## Properties

### **CorrelationId**

Identifies the saga instance uniquely, and is the primary correlation
 for the instance. While the setter is not typically called, it is there
 to support persistence consistently across implementations.

```csharp
public abstract Guid CorrelationId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
