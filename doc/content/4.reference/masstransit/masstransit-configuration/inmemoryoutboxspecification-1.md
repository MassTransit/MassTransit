---

title: InMemoryOutboxSpecification<T>

---

# InMemoryOutboxSpecification\<T\>

Namespace: MassTransit.Configuration

```csharp
public class InMemoryOutboxSpecification<T> : IPipeSpecification<ConsumeContext<T>>, ISpecification, IOutboxConfigurator
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryOutboxSpecification\<T\>](../masstransit-configuration/inmemoryoutboxspecification-1)<br/>
Implements [IPipeSpecification\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IOutboxConfigurator](../masstransit/ioutboxconfigurator)

## Properties

### **ConcurrentMessageDelivery**

```csharp
public bool ConcurrentMessageDelivery { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **InMemoryOutboxSpecification(IRegistrationContext)**

```csharp
public InMemoryOutboxSpecification(IRegistrationContext context)
```

#### Parameters

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

### **InMemoryOutboxSpecification(ISetScopedConsumeContext)**

```csharp
public InMemoryOutboxSpecification(ISetScopedConsumeContext setter)
```

#### Parameters

`setter` [ISetScopedConsumeContext](../masstransit/isetscopedconsumecontext)<br/>

## Methods

### **Apply(IPipeBuilder\<ConsumeContext\<T\>\>)**

```csharp
public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
```

#### Parameters

`builder` [IPipeBuilder\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
