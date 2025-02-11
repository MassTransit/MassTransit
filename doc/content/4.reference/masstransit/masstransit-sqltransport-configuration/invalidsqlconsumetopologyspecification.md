---

title: InvalidSqlConsumeTopologySpecification

---

# InvalidSqlConsumeTopologySpecification

Namespace: MassTransit.SqlTransport.Configuration

```csharp
public class InvalidSqlConsumeTopologySpecification : ISqlConsumeTopologySpecification, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InvalidSqlConsumeTopologySpecification](../masstransit-sqltransport-configuration/invalidsqlconsumetopologyspecification)<br/>
Implements [ISqlConsumeTopologySpecification](../masstransit-sqltransport-configuration/isqlconsumetopologyspecification), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **InvalidSqlConsumeTopologySpecification(String, String)**

```csharp
public InvalidSqlConsumeTopologySpecification(string key, string message)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Apply(IReceiveEndpointBrokerTopologyBuilder)**

```csharp
public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
```

#### Parameters

`builder` [IReceiveEndpointBrokerTopologyBuilder](../masstransit-sqltransport-topology/ireceiveendpointbrokertopologybuilder)<br/>
