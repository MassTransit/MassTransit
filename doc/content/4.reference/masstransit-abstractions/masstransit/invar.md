---

title: InVar

---

# InVar

Namespace: MassTransit

Variables, which can be used for message initialization

```csharp
public static class InVar
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InVar](../masstransit/invar)

## Properties

### **Timestamp**

Generates the current timestamp, in UTC, which can be used to initialize properties
 in the message with a consistent value

```csharp
public static TimestampVariable Timestamp { get; }
```

#### Property Value

[TimestampVariable](../masstransit-initializers-variables/timestampvariable)<br/>

### **Id**

Generates a new identifier, and maintains that identifier for the entire message initializer lifetime,
 so that subsequent uses of the identifier return the same value. There are multiple aliases for the same
 identifier, so that property names are automatically inferred (Id, CorrelationId, etc.).

```csharp
public static IdVariable Id { get; }
```

#### Property Value

[IdVariable](../masstransit-initializers-variables/idvariable)<br/>

### **CorrelationId**

Generates a new identifier, and maintains that identifier for the entire message initializer lifetime,
 so that subsequent uses of the identifier return the same value. There are multiple aliases for the same
 identifier, so that property names are automatically inferred (Id, CorrelationId, etc.).

```csharp
public static IdVariable CorrelationId { get; }
```

#### Property Value

[IdVariable](../masstransit-initializers-variables/idvariable)<br/>
