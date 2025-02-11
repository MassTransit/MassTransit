---

title: ConsumeContextRescuePipeSpecification<T>

---

# ConsumeContextRescuePipeSpecification\<T\>

Namespace: MassTransit.Configuration

```csharp
public class ConsumeContextRescuePipeSpecification<T> : ExceptionSpecification, IExceptionConfigurator, IPipeSpecification<ConsumeContext<T>>, ISpecification
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExceptionSpecification](../../masstransit-abstractions/masstransit-configuration/exceptionspecification) → [ConsumeContextRescuePipeSpecification\<T\>](../masstransit-configuration/consumecontextrescuepipespecification-1)<br/>
Implements [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator), [IPipeSpecification\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **ConsumeContextRescuePipeSpecification(IPipe\<ExceptionConsumeContext\<T\>\>)**

```csharp
public ConsumeContextRescuePipeSpecification(IPipe<ExceptionConsumeContext<T>> rescuePipe)
```

#### Parameters

`rescuePipe` [IPipe\<ExceptionConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

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
