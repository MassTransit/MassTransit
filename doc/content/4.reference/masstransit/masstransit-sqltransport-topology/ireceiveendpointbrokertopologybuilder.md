---

title: IReceiveEndpointBrokerTopologyBuilder

---

# IReceiveEndpointBrokerTopologyBuilder

Namespace: MassTransit.SqlTransport.Topology

A unique builder context should be created for each specification, so that the items added
 by it can be combined together into a group - so that if a subsequent specification yanks
 something that conflicts, the system can yank the group or warn that it's impacted.

```csharp
public interface IReceiveEndpointBrokerTopologyBuilder : IBrokerTopologyBuilder
```

Implements [IBrokerTopologyBuilder](../masstransit-sqltransport-topology/ibrokertopologybuilder)

## Properties

### **Queue**

A handle to the consuming queue

```csharp
public abstract QueueHandle Queue { get; }
```

#### Property Value

[QueueHandle](../masstransit-sqltransport-topology/queuehandle)<br/>
