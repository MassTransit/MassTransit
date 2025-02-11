---

title: PartitionKeyExtensions

---

# PartitionKeyExtensions

Namespace: MassTransit

```csharp
public static class PartitionKeyExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PartitionKeyExtensions](../masstransit/partitionkeyextensions)

## Methods

### **PartitionKey(ConsumeContext)**

```csharp
public static string PartitionKey(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **PartitionKey(SendContext)**

```csharp
public static string PartitionKey(SendContext context)
```

#### Parameters

`context` [SendContext](../masstransit/sendcontext)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **SetPartitionKey(SendContext, String)**

Sets the routing key for this message

```csharp
public static void SetPartitionKey(SendContext context, string routingKey)
```

#### Parameters

`context` [SendContext](../masstransit/sendcontext)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The routing key for this message

### **TrySetPartitionKey(SendContext, String)**

Sets the routing key for this message

```csharp
public static bool TrySetPartitionKey(SendContext context, string routingKey)
```

#### Parameters

`context` [SendContext](../masstransit/sendcontext)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The routing key for this message

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
