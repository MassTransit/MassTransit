---

title: ConsumerSplitFilterSpecification<TConsumer, TMessage>

---

# ConsumerSplitFilterSpecification\<TConsumer, TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class ConsumerSplitFilterSpecification<TConsumer, TMessage> : IPipeSpecification<ConsumerConsumeContext<TConsumer, TMessage>>, ISpecification
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerSplitFilterSpecification\<TConsumer, TMessage\>](../masstransit-configuration/consumersplitfilterspecification-2)<br/>
Implements [IPipeSpecification\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **ConsumerSplitFilterSpecification(IPipeSpecification\<ConsumerConsumeContext\<TConsumer\>\>)**

```csharp
public ConsumerSplitFilterSpecification(IPipeSpecification<ConsumerConsumeContext<TConsumer>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ConsumerConsumeContext\<TConsumer\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

## Methods

### **Apply(IPipeBuilder\<ConsumerConsumeContext\<TConsumer, TMessage\>\>)**

```csharp
public void Apply(IPipeBuilder<ConsumerConsumeContext<TConsumer, TMessage>> builder)
```

#### Parameters

`builder` [IPipeBuilder\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
