---

title: PublishEndpointBrokerTopologyBuilder

---

# PublishEndpointBrokerTopologyBuilder

Namespace: MassTransit.SqlTransport.Topology

```csharp
public class PublishEndpointBrokerTopologyBuilder : BrokerTopologyBuilder, IPublishEndpointBrokerTopologyBuilder, IBrokerTopologyBuilder
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BrokerTopologyBuilder](../masstransit-sqltransport-topology/brokertopologybuilder) → [PublishEndpointBrokerTopologyBuilder](../masstransit-sqltransport-topology/publishendpointbrokertopologybuilder)<br/>
Implements [IPublishEndpointBrokerTopologyBuilder](../masstransit-sqltransport-topology/ipublishendpointbrokertopologybuilder), [IBrokerTopologyBuilder](../masstransit-sqltransport-topology/ibrokertopologybuilder)

## Properties

### **Topic**

The topic to which the published message is sent

```csharp
public TopicHandle Topic { get; set; }
```

#### Property Value

[TopicHandle](../masstransit-sqltransport-topology/topichandle)<br/>

## Constructors

### **PublishEndpointBrokerTopologyBuilder()**

```csharp
public PublishEndpointBrokerTopologyBuilder()
```

## Methods

### **CreateImplementedBuilder()**

```csharp
public IPublishEndpointBrokerTopologyBuilder CreateImplementedBuilder()
```

#### Returns

[IPublishEndpointBrokerTopologyBuilder](../masstransit-sqltransport-topology/ipublishendpointbrokertopologybuilder)<br/>
