---

title: ReceiveEndpointBrokerTopologyBuilder

---

# ReceiveEndpointBrokerTopologyBuilder

Namespace: MassTransit.SqlTransport.Topology

```csharp
public class ReceiveEndpointBrokerTopologyBuilder : BrokerTopologyBuilder, IReceiveEndpointBrokerTopologyBuilder, IBrokerTopologyBuilder
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BrokerTopologyBuilder](../masstransit-sqltransport-topology/brokertopologybuilder) → [ReceiveEndpointBrokerTopologyBuilder](../masstransit-sqltransport-topology/receiveendpointbrokertopologybuilder)<br/>
Implements [IReceiveEndpointBrokerTopologyBuilder](../masstransit-sqltransport-topology/ireceiveendpointbrokertopologybuilder), [IBrokerTopologyBuilder](../masstransit-sqltransport-topology/ibrokertopologybuilder)

## Properties

### **Queue**

```csharp
public QueueHandle Queue { get; }
```

#### Property Value

[QueueHandle](../masstransit-sqltransport-topology/queuehandle)<br/>

## Constructors

### **ReceiveEndpointBrokerTopologyBuilder(ReceiveSettings)**

```csharp
public ReceiveEndpointBrokerTopologyBuilder(ReceiveSettings settings)
```

#### Parameters

`settings` [ReceiveSettings](../masstransit-sqltransport/receivesettings)<br/>
