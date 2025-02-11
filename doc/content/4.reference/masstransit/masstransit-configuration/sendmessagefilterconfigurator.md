---

title: SendMessageFilterConfigurator

---

# SendMessageFilterConfigurator

Namespace: MassTransit.Configuration

```csharp
public class SendMessageFilterConfigurator : IMessageFilterConfigurator, IMessageTypeFilterConfigurator
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendMessageFilterConfigurator](../masstransit-configuration/sendmessagefilterconfigurator)<br/>
Implements [IMessageFilterConfigurator](../../masstransit-abstractions/masstransit/imessagefilterconfigurator), [IMessageTypeFilterConfigurator](../../masstransit-abstractions/masstransit/imessagetypefilterconfigurator)

## Properties

### **Filter**

```csharp
public CompositeFilter<SendContext> Filter { get; }
```

#### Property Value

[CompositeFilter\<SendContext\>](../masstransit-configuration/compositefilter-1)<br/>

## Constructors

### **SendMessageFilterConfigurator()**

```csharp
public SendMessageFilterConfigurator()
```
