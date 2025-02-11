---

title: ISagaVersion

---

# ISagaVersion

Namespace: MassTransit

For saga repositories that use an incrementing version

```csharp
public interface ISagaVersion : ISaga
```

Implements [ISaga](../masstransit/isaga)

## Properties

### **Version**

```csharp
public abstract int Version { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
