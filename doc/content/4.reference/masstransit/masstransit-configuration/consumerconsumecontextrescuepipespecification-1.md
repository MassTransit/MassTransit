---

title: ConsumerConsumeContextRescuePipeSpecification<T>

---

# ConsumerConsumeContextRescuePipeSpecification\<T\>

Namespace: MassTransit.Configuration

```csharp
public class ConsumerConsumeContextRescuePipeSpecification<T> : ExceptionSpecification, IExceptionConfigurator, IPipeSpecification<ConsumerConsumeContext<T>>, ISpecification
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExceptionSpecification](../../masstransit-abstractions/masstransit-configuration/exceptionspecification) → [ConsumerConsumeContextRescuePipeSpecification\<T\>](../masstransit-configuration/consumerconsumecontextrescuepipespecification-1)<br/>
Implements [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator), [IPipeSpecification\<ConsumerConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **ConsumerConsumeContextRescuePipeSpecification(IPipe\<ExceptionConsumerConsumeContext\<T\>\>)**

```csharp
public ConsumerConsumeContextRescuePipeSpecification(IPipe<ExceptionConsumerConsumeContext<T>> rescuePipe)
```

#### Parameters

`rescuePipe` [IPipe\<ExceptionConsumerConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

## Methods

### **Apply(IPipeBuilder\<ConsumerConsumeContext\<T\>\>)**

```csharp
public void Apply(IPipeBuilder<ConsumerConsumeContext<T>> builder)
```

#### Parameters

`builder` [IPipeBuilder\<ConsumerConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
