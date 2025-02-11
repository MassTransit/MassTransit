---

title: DeadLetterSettings

---

# DeadLetterSettings

Namespace: MassTransit.SqlTransport

```csharp
public interface DeadLetterSettings : EntitySettings
```

Implements [EntitySettings](../masstransit-sqltransport/entitysettings)

## Methods

### **GetBrokerTopology()**

Return the BrokerTopology to apply at startup (to create exchange and queue if binding is specified)

```csharp
BrokerTopology GetBrokerTopology()
```

#### Returns

[BrokerTopology](../masstransit-sqltransport-topology/brokertopology)<br/>
