---

title: SqlBusTopology

---

# SqlBusTopology

Namespace: MassTransit.SqlTransport.Topology

```csharp
public class SqlBusTopology : BusTopology, IBusTopology, ISqlBusTopology
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BusTopology](../masstransit-transports/bustopology) → [SqlBusTopology](../masstransit-sqltransport-topology/sqlbustopology)<br/>
Implements [IBusTopology](../../masstransit-abstractions/masstransit/ibustopology), [ISqlBusTopology](../masstransit/isqlbustopology)

## Properties

### **PublishTopology**

```csharp
public IPublishTopology PublishTopology { get; }
```

#### Property Value

[IPublishTopology](../../masstransit-abstractions/masstransit/ipublishtopology)<br/>

### **SendTopology**

```csharp
public ISendTopology SendTopology { get; }
```

#### Property Value

[ISendTopology](../../masstransit-abstractions/masstransit/isendtopology)<br/>

## Constructors

### **SqlBusTopology(ISqlHostConfiguration, ISqlTopologyConfiguration)**

```csharp
public SqlBusTopology(ISqlHostConfiguration hostConfiguration, ISqlTopologyConfiguration configuration)
```

#### Parameters

`hostConfiguration` [ISqlHostConfiguration](../masstransit-sqltransport-configuration/isqlhostconfiguration)<br/>

`configuration` [ISqlTopologyConfiguration](../masstransit-sqltransport-configuration/isqltopologyconfiguration)<br/>
