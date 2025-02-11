---

title: ProbeContextExtensions

---

# ProbeContextExtensions

Namespace: MassTransit

```csharp
public static class ProbeContextExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ProbeContextExtensions](../masstransit/probecontextextensions)

## Methods

### **CreateFilterScope(ProbeContext, String)**

```csharp
public static ProbeContext CreateFilterScope(ProbeContext context, string filterType)
```

#### Parameters

`context` [ProbeContext](../masstransit/probecontext)<br/>

`filterType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ProbeContext](../masstransit/probecontext)<br/>

### **CreateConsumerFactoryScope\<TConsumer\>(ProbeContext, String)**

```csharp
public static ProbeContext CreateConsumerFactoryScope<TConsumer>(ProbeContext context, string source)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`context` [ProbeContext](../masstransit/probecontext)<br/>

`source` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ProbeContext](../masstransit/probecontext)<br/>

### **CreateMessageScope(ProbeContext, String)**

```csharp
public static ProbeContext CreateMessageScope(ProbeContext context, string messageType)
```

#### Parameters

`context` [ProbeContext](../masstransit/probecontext)<br/>

`messageType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ProbeContext](../masstransit/probecontext)<br/>
