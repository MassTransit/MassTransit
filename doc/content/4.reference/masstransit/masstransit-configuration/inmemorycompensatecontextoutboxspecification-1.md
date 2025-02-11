---

title: InMemoryCompensateContextOutboxSpecification<TArguments>

---

# InMemoryCompensateContextOutboxSpecification\<TArguments\>

Namespace: MassTransit.Configuration

```csharp
public class InMemoryCompensateContextOutboxSpecification<TArguments> : IPipeSpecification<CompensateContext<TArguments>>, ISpecification, IOutboxConfigurator
```

#### Type Parameters

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryCompensateContextOutboxSpecification\<TArguments\>](../masstransit-configuration/inmemorycompensatecontextoutboxspecification-1)<br/>
Implements [IPipeSpecification\<CompensateContext\<TArguments\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IOutboxConfigurator](../masstransit/ioutboxconfigurator)

## Properties

### **ConcurrentMessageDelivery**

```csharp
public bool ConcurrentMessageDelivery { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **InMemoryCompensateContextOutboxSpecification(IRegistrationContext)**

```csharp
public InMemoryCompensateContextOutboxSpecification(IRegistrationContext context)
```

#### Parameters

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

### **InMemoryCompensateContextOutboxSpecification(ISetScopedConsumeContext)**

```csharp
public InMemoryCompensateContextOutboxSpecification(ISetScopedConsumeContext setter)
```

#### Parameters

`setter` [ISetScopedConsumeContext](../masstransit/isetscopedconsumecontext)<br/>

## Methods

### **Apply(IPipeBuilder\<CompensateContext\<TArguments\>\>)**

```csharp
public void Apply(IPipeBuilder<CompensateContext<TArguments>> builder)
```

#### Parameters

`builder` [IPipeBuilder\<CompensateContext\<TArguments\>\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
