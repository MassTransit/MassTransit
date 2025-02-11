---

title: InvalidInMemoryConsumeTopologySpecification

---

# InvalidInMemoryConsumeTopologySpecification

Namespace: MassTransit.InMemoryTransport.Configuration

```csharp
public class InvalidInMemoryConsumeTopologySpecification : IInMemoryConsumeTopologySpecification, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InvalidInMemoryConsumeTopologySpecification](../masstransit-inmemorytransport-configuration/invalidinmemoryconsumetopologyspecification)<br/>
Implements [IInMemoryConsumeTopologySpecification](../masstransit-inmemorytransport-configuration/iinmemoryconsumetopologyspecification), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **InvalidInMemoryConsumeTopologySpecification(String, String)**

```csharp
public InvalidInMemoryConsumeTopologySpecification(string key, string message)
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

### **Apply(IMessageFabricConsumeTopologyBuilder)**

```csharp
public void Apply(IMessageFabricConsumeTopologyBuilder builder)
```

#### Parameters

`builder` [IMessageFabricConsumeTopologyBuilder](../masstransit-configuration/imessagefabricconsumetopologybuilder)<br/>
