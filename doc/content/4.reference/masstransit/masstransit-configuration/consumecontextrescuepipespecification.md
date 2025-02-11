---

title: ConsumeContextRescuePipeSpecification

---

# ConsumeContextRescuePipeSpecification

Namespace: MassTransit.Configuration

```csharp
public class ConsumeContextRescuePipeSpecification : ExceptionSpecification, IExceptionConfigurator, IPipeSpecification<ConsumeContext>, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExceptionSpecification](../../masstransit-abstractions/masstransit-configuration/exceptionspecification) → [ConsumeContextRescuePipeSpecification](../masstransit-configuration/consumecontextrescuepipespecification)<br/>
Implements [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator), [IPipeSpecification\<ConsumeContext\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **ConsumeContextRescuePipeSpecification(IPipe\<ExceptionConsumeContext\>)**

```csharp
public ConsumeContextRescuePipeSpecification(IPipe<ExceptionConsumeContext> rescuePipe)
```

#### Parameters

`rescuePipe` [IPipe\<ExceptionConsumeContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

## Methods

### **Apply(IPipeBuilder\<ConsumeContext\>)**

```csharp
public void Apply(IPipeBuilder<ConsumeContext> builder)
```

#### Parameters

`builder` [IPipeBuilder\<ConsumeContext\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
