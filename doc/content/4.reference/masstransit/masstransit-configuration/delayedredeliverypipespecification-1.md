---

title: DelayedRedeliveryPipeSpecification<TMessage>

---

# DelayedRedeliveryPipeSpecification\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class DelayedRedeliveryPipeSpecification<TMessage> : IPipeSpecification<ConsumeContext<TMessage>>, ISpecification, IRedeliveryPipeSpecification
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DelayedRedeliveryPipeSpecification\<TMessage\>](../masstransit-configuration/delayedredeliverypipespecification-1)<br/>
Implements [IPipeSpecification\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IRedeliveryPipeSpecification](../masstransit-configuration/iredeliverypipespecification)

## Properties

### **Options**

```csharp
public RedeliveryOptions Options { get; set; }
```

#### Property Value

[RedeliveryOptions](../../masstransit-abstractions/masstransit/redeliveryoptions)<br/>

## Constructors

### **DelayedRedeliveryPipeSpecification()**

```csharp
public DelayedRedeliveryPipeSpecification()
```

## Methods

### **Apply(IPipeBuilder\<ConsumeContext\<TMessage\>\>)**

```csharp
public void Apply(IPipeBuilder<ConsumeContext<TMessage>> builder)
```

#### Parameters

`builder` [IPipeBuilder\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
