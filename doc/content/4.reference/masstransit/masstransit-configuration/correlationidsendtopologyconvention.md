---

title: CorrelationIdSendTopologyConvention

---

# CorrelationIdSendTopologyConvention

Namespace: MassTransit.Configuration

Looks for a property that can be used as a CorrelationId message header, and
 applies a filter to set it on message send if available

```csharp
public class CorrelationIdSendTopologyConvention : ISendTopologyConvention, IMessageSendTopologyConvention
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CorrelationIdSendTopologyConvention](../masstransit-configuration/correlationidsendtopologyconvention)<br/>
Implements [ISendTopologyConvention](../../masstransit-abstractions/masstransit-configuration/isendtopologyconvention), [IMessageSendTopologyConvention](../../masstransit-abstractions/masstransit-configuration/imessagesendtopologyconvention)

## Constructors

### **CorrelationIdSendTopologyConvention()**

```csharp
public CorrelationIdSendTopologyConvention()
```

## Methods

### **TryGetMessageSendTopologyConvention\<T\>(IMessageSendTopologyConvention\<T\>)**

```csharp
public bool TryGetMessageSendTopologyConvention<T>(out IMessageSendTopologyConvention<T> convention)
```

#### Type Parameters

`T`<br/>

#### Parameters

`convention` [IMessageSendTopologyConvention\<T\>](../../masstransit-abstractions/masstransit-configuration/imessagesendtopologyconvention-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
