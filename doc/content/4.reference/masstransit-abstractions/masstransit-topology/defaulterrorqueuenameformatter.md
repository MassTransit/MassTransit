---

title: DefaultErrorQueueNameFormatter

---

# DefaultErrorQueueNameFormatter

Namespace: MassTransit.Topology

```csharp
public class DefaultErrorQueueNameFormatter : IErrorQueueNameFormatter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DefaultErrorQueueNameFormatter](../masstransit-topology/defaulterrorqueuenameformatter)<br/>
Implements [IErrorQueueNameFormatter](../masstransit/ierrorqueuenameformatter)

## Fields

### **Instance**

```csharp
public static IErrorQueueNameFormatter Instance;
```

## Constructors

### **DefaultErrorQueueNameFormatter()**

```csharp
public DefaultErrorQueueNameFormatter()
```

## Methods

### **FormatErrorQueueName(String)**

```csharp
public string FormatErrorQueueName(string queueName)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
