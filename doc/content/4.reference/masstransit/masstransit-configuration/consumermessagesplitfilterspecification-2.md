---

title: ConsumerMessageSplitFilterSpecification<TConsumer, TMessage>

---

# ConsumerMessageSplitFilterSpecification\<TConsumer, TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class ConsumerMessageSplitFilterSpecification<TConsumer, TMessage> : IPipeSpecification<ConsumerConsumeContext<TConsumer, TMessage>>, ISpecification
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerMessageSplitFilterSpecification\<TConsumer, TMessage\>](../masstransit-configuration/consumermessagesplitfilterspecification-2)<br/>
Implements [IPipeSpecification\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **ConsumerMessageSplitFilterSpecification(IPipeSpecification\<ConsumeContext\<TMessage\>\>)**

```csharp
public ConsumerMessageSplitFilterSpecification(IPipeSpecification<ConsumeContext<TMessage>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

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
