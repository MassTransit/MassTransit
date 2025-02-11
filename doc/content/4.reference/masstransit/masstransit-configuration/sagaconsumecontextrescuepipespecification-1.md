---

title: SagaConsumeContextRescuePipeSpecification<T>

---

# SagaConsumeContextRescuePipeSpecification\<T\>

Namespace: MassTransit.Configuration

```csharp
public class SagaConsumeContextRescuePipeSpecification<T> : ExceptionSpecification, IExceptionConfigurator, IPipeSpecification<SagaConsumeContext<T>>, ISpecification
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExceptionSpecification](../../masstransit-abstractions/masstransit-configuration/exceptionspecification) → [SagaConsumeContextRescuePipeSpecification\<T\>](../masstransit-configuration/sagaconsumecontextrescuepipespecification-1)<br/>
Implements [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator), [IPipeSpecification\<SagaConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **SagaConsumeContextRescuePipeSpecification(IPipe\<ExceptionSagaConsumeContext\<T\>\>)**

```csharp
public SagaConsumeContextRescuePipeSpecification(IPipe<ExceptionSagaConsumeContext<T>> rescuePipe)
```

#### Parameters

`rescuePipe` [IPipe\<ExceptionSagaConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

## Methods

### **Apply(IPipeBuilder\<SagaConsumeContext\<T\>\>)**

```csharp
public void Apply(IPipeBuilder<SagaConsumeContext<T>> builder)
```

#### Parameters

`builder` [IPipeBuilder\<SagaConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
