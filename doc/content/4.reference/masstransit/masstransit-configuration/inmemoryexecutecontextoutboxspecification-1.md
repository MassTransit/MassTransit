---

title: InMemoryExecuteContextOutboxSpecification<TArguments>

---

# InMemoryExecuteContextOutboxSpecification\<TArguments\>

Namespace: MassTransit.Configuration

```csharp
public class InMemoryExecuteContextOutboxSpecification<TArguments> : IPipeSpecification<ExecuteContext<TArguments>>, ISpecification, IOutboxConfigurator
```

#### Type Parameters

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryExecuteContextOutboxSpecification\<TArguments\>](../masstransit-configuration/inmemoryexecutecontextoutboxspecification-1)<br/>
Implements [IPipeSpecification\<ExecuteContext\<TArguments\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IOutboxConfigurator](../masstransit/ioutboxconfigurator)

## Properties

### **ConcurrentMessageDelivery**

```csharp
public bool ConcurrentMessageDelivery { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **InMemoryExecuteContextOutboxSpecification(IRegistrationContext)**

```csharp
public InMemoryExecuteContextOutboxSpecification(IRegistrationContext context)
```

#### Parameters

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

### **InMemoryExecuteContextOutboxSpecification(ISetScopedConsumeContext)**

```csharp
public InMemoryExecuteContextOutboxSpecification(ISetScopedConsumeContext setter)
```

#### Parameters

`setter` [ISetScopedConsumeContext](../masstransit/isetscopedconsumecontext)<br/>

## Methods

### **Apply(IPipeBuilder\<ExecuteContext\<TArguments\>\>)**

```csharp
public void Apply(IPipeBuilder<ExecuteContext<TArguments>> builder)
```

#### Parameters

`builder` [IPipeBuilder\<ExecuteContext\<TArguments\>\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
