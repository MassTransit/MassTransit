---

title: SqlTopologyConfiguration

---

# SqlTopologyConfiguration

Namespace: MassTransit.SqlTransport.Configuration

```csharp
public class SqlTopologyConfiguration : ISqlTopologyConfiguration, ITopologyConfiguration, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlTopologyConfiguration](../masstransit-sqltransport-configuration/sqltopologyconfiguration)<br/>
Implements [ISqlTopologyConfiguration](../masstransit-sqltransport-configuration/isqltopologyconfiguration), [ITopologyConfiguration](../masstransit-configuration/itopologyconfiguration), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **SqlTopologyConfiguration(IMessageTopologyConfigurator)**

```csharp
public SqlTopologyConfiguration(IMessageTopologyConfigurator messageTopology)
```

#### Parameters

`messageTopology` [IMessageTopologyConfigurator](../../masstransit-abstractions/masstransit-configuration/imessagetopologyconfigurator)<br/>

### **SqlTopologyConfiguration(ISqlTopologyConfiguration)**

```csharp
public SqlTopologyConfiguration(ISqlTopologyConfiguration topologyConfiguration)
```

#### Parameters

`topologyConfiguration` [ISqlTopologyConfiguration](../masstransit-sqltransport-configuration/isqltopologyconfiguration)<br/>

## Methods

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
