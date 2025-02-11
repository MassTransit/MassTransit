---

title: RoutingKeyExtensions

---

# RoutingKeyExtensions

Namespace: MassTransit

```csharp
public static class RoutingKeyExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingKeyExtensions](../masstransit/routingkeyextensions)

## Methods

### **RoutingKey(ConsumeContext)**

```csharp
public static string RoutingKey(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **RoutingKey(SendContext)**

```csharp
public static string RoutingKey(SendContext context)
```

#### Parameters

`context` [SendContext](../masstransit/sendcontext)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **SetRoutingKey(SendContext, String)**

Sets the routing key for this message

```csharp
public static void SetRoutingKey(SendContext context, string routingKey)
```

#### Parameters

`context` [SendContext](../masstransit/sendcontext)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The routing key for this message

### **TrySetRoutingKey(SendContext, String)**

Sets the routing key for this message

```csharp
public static bool TrySetRoutingKey(SendContext context, string routingKey)
```

#### Parameters

`context` [SendContext](../masstransit/sendcontext)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The routing key for this message

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
