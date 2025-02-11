---

title: IRoutingSlipBuilder

---

# IRoutingSlipBuilder

Namespace: MassTransit

```csharp
public interface IRoutingSlipBuilder : IItineraryBuilder
```

Implements [IItineraryBuilder](../masstransit/iitinerarybuilder)

## Methods

### **Build()**

Builds the routing slip using the current state of the builder

```csharp
RoutingSlip Build()
```

#### Returns

[RoutingSlip](../masstransit-courier-contracts/routingslip)<br/>
The RoutingSlip
