---

title: DeadLetterPipeSpecification

---

# DeadLetterPipeSpecification

Namespace: MassTransit.Configuration

```csharp
public class DeadLetterPipeSpecification : IPipeSpecification<ReceiveContext>, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DeadLetterPipeSpecification](../masstransit-configuration/deadletterpipespecification)<br/>
Implements [IPipeSpecification\<ReceiveContext\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **DeadLetterPipeSpecification(IPipe\<ReceiveContext\>)**

```csharp
public DeadLetterPipeSpecification(IPipe<ReceiveContext> deadLetterPipe)
```

#### Parameters

`deadLetterPipe` [IPipe\<ReceiveContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

## Methods

### **Apply(IPipeBuilder\<ReceiveContext\>)**

```csharp
public void Apply(IPipeBuilder<ReceiveContext> builder)
```

#### Parameters

`builder` [IPipeBuilder\<ReceiveContext\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
