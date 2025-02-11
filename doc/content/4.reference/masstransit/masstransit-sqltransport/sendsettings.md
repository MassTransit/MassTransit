---

title: SendSettings

---

# SendSettings

Namespace: MassTransit.SqlTransport

```csharp
public interface SendSettings : EntitySettings
```

Implements [EntitySettings](../masstransit-sqltransport/entitysettings)

## Methods

### **GetSendAddress(Uri)**

Returns the send address for the settings

```csharp
SqlEndpointAddress GetSendAddress(Uri hostAddress)
```

#### Parameters

`hostAddress` Uri<br/>

#### Returns

[SqlEndpointAddress](../masstransit/sqlendpointaddress)<br/>

### **GetBrokerTopology()**

Return the BrokerTopology to apply at startup (to create exchange and queue if binding is specified)

```csharp
BrokerTopology GetBrokerTopology()
```

#### Returns

[BrokerTopology](../masstransit-sqltransport-topology/brokertopology)<br/>
