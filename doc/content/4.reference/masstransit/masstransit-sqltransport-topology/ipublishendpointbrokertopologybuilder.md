---

title: IPublishEndpointBrokerTopologyBuilder

---

# IPublishEndpointBrokerTopologyBuilder

Namespace: MassTransit.SqlTransport.Topology

A builder for creating the topology when publishing a message

```csharp
public interface IPublishEndpointBrokerTopologyBuilder : IBrokerTopologyBuilder
```

Implements [IBrokerTopologyBuilder](../masstransit-sqltransport-topology/ibrokertopologybuilder)

## Properties

### **Topic**

The exchange to which the message is published

```csharp
public abstract TopicHandle Topic { get; set; }
```

#### Property Value

[TopicHandle](../masstransit-sqltransport-topology/topichandle)<br/>

## Methods

### **CreateImplementedBuilder()**

```csharp
IPublishEndpointBrokerTopologyBuilder CreateImplementedBuilder()
```

#### Returns

[IPublishEndpointBrokerTopologyBuilder](../masstransit-sqltransport-topology/ipublishendpointbrokertopologybuilder)<br/>
