---

title: IMessagePublishTopologyConfigurator

---

# IMessagePublishTopologyConfigurator

Namespace: MassTransit

```csharp
public interface IMessagePublishTopologyConfigurator : IMessagePublishTopology, ISpecification
```

Implements [IMessagePublishTopology](../masstransit/imessagepublishtopology), [ISpecification](../masstransit/ispecification)

## Properties

### **Exclude**

Exclude the message type from being created as a topic/exchange.

```csharp
public abstract bool Exclude { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **TryAddConvention(IPublishTopologyConvention)**

```csharp
bool TryAddConvention(IPublishTopologyConvention convention)
```

#### Parameters

`convention` [IPublishTopologyConvention](../masstransit-configuration/ipublishtopologyconvention)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
