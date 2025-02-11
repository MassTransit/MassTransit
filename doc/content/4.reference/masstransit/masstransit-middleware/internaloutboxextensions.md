---

title: InternalOutboxExtensions

---

# InternalOutboxExtensions

Namespace: MassTransit.Middleware

```csharp
public static class InternalOutboxExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InternalOutboxExtensions](../masstransit-middleware/internaloutboxextensions)

## Methods

### **SkipOutbox(ISendEndpoint)**

```csharp
internal static ISendEndpoint SkipOutbox(ISendEndpoint endpoint)
```

#### Parameters

`endpoint` [ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint)<br/>

#### Returns

[ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint)<br/>

### **SkipOutbox(ConsumeContext)**

```csharp
public static ConsumeContext SkipOutbox(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

#### Returns

[ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>
