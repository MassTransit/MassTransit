---

title: SagaFilterSpecification<TSaga, TMessage>

---

# SagaFilterSpecification\<TSaga, TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class SagaFilterSpecification<TSaga, TMessage> : IPipeSpecification<SagaConsumeContext<TSaga, TMessage>>, ISpecification
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaFilterSpecification\<TSaga, TMessage\>](../masstransit-configuration/sagafilterspecification-2)<br/>
Implements [IPipeSpecification\<SagaConsumeContext\<TSaga, TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **SagaFilterSpecification(IFilter\<SagaConsumeContext\<TSaga\>\>)**

```csharp
public SagaFilterSpecification(IFilter<SagaConsumeContext<TSaga>> filter)
```

#### Parameters

`filter` [IFilter\<SagaConsumeContext\<TSaga\>\>](../../masstransit-abstractions/masstransit/ifilter-1)<br/>

## Methods

### **Apply(IPipeBuilder\<SagaConsumeContext\<TSaga, TMessage\>\>)**

```csharp
public void Apply(IPipeBuilder<SagaConsumeContext<TSaga, TMessage>> builder)
```

#### Parameters

`builder` [IPipeBuilder\<SagaConsumeContext\<TSaga, TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
