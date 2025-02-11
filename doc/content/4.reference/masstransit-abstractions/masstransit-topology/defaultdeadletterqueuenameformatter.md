---

title: DefaultDeadLetterQueueNameFormatter

---

# DefaultDeadLetterQueueNameFormatter

Namespace: MassTransit.Topology

```csharp
public class DefaultDeadLetterQueueNameFormatter : IDeadLetterQueueNameFormatter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DefaultDeadLetterQueueNameFormatter](../masstransit-topology/defaultdeadletterqueuenameformatter)<br/>
Implements [IDeadLetterQueueNameFormatter](../masstransit/ideadletterqueuenameformatter)

## Fields

### **Instance**

```csharp
public static IDeadLetterQueueNameFormatter Instance;
```

## Constructors

### **DefaultDeadLetterQueueNameFormatter()**

```csharp
public DefaultDeadLetterQueueNameFormatter()
```

## Methods

### **FormatDeadLetterQueueName(String)**

```csharp
public string FormatDeadLetterQueueName(string queueName)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
