---

title: SagaConnectorCache<TSaga>

---

# SagaConnectorCache\<TSaga\>

Namespace: MassTransit.Configuration

Caches the saga connectors for the saga

```csharp
public class SagaConnectorCache<TSaga> : ISagaConnectorCache
```

#### Type Parameters

`TSaga`<br/>
The saga type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaConnectorCache\<TSaga\>](../masstransit-configuration/sagaconnectorcache-1)<br/>
Implements [ISagaConnectorCache](../masstransit-configuration/isagaconnectorcache)

## Properties

### **Connector**

```csharp
public static ISagaConnector Connector { get; }
```

#### Property Value

[ISagaConnector](../masstransit-configuration/isagaconnector)<br/>
