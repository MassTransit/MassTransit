---

title: SqlSendContextExtensions

---

# SqlSendContextExtensions

Namespace: MassTransit

```csharp
public static class SqlSendContextExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlSendContextExtensions](../masstransit/sqlsendcontextextensions)

## Methods

### **SetPriority(SendContext, Int16)**

Sets the message priority (default: 100)

```csharp
public static void SetPriority(SendContext context, short priority)
```

#### Parameters

`context` [SendContext](../../masstransit-abstractions/masstransit/sendcontext)<br/>

`priority` [Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

### **TrySetPriority(SendContext, Int16)**

Sets the message priority (default: 100)

```csharp
public static bool TrySetPriority(SendContext context, short priority)
```

#### Parameters

`context` [SendContext](../../masstransit-abstractions/masstransit/sendcontext)<br/>

`priority` [Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
