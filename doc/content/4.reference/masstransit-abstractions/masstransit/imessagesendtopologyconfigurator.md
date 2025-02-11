---

title: IMessageSendTopologyConfigurator

---

# IMessageSendTopologyConfigurator

Namespace: MassTransit

```csharp
public interface IMessageSendTopologyConfigurator : ISpecification
```

Implements [ISpecification](../masstransit/ispecification)

## Methods

### **TryAddConvention(ISendTopologyConvention)**

```csharp
bool TryAddConvention(ISendTopologyConvention convention)
```

#### Parameters

`convention` [ISendTopologyConvention](../masstransit-configuration/isendtopologyconvention)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
