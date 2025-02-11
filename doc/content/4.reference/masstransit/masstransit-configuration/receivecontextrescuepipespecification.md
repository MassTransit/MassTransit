---

title: ReceiveContextRescuePipeSpecification

---

# ReceiveContextRescuePipeSpecification

Namespace: MassTransit.Configuration

```csharp
public class ReceiveContextRescuePipeSpecification : ExceptionSpecification, IExceptionConfigurator, IPipeSpecification<ReceiveContext>, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExceptionSpecification](../../masstransit-abstractions/masstransit-configuration/exceptionspecification) → [ReceiveContextRescuePipeSpecification](../masstransit-configuration/receivecontextrescuepipespecification)<br/>
Implements [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator), [IPipeSpecification\<ReceiveContext\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **ReceiveContextRescuePipeSpecification(IPipe\<ExceptionReceiveContext\>)**

```csharp
public ReceiveContextRescuePipeSpecification(IPipe<ExceptionReceiveContext> rescuePipe)
```

#### Parameters

`rescuePipe` [IPipe\<ExceptionReceiveContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

## Methods

### **Apply(IPipeBuilder\<ReceiveContext\>)**

```csharp
public void Apply(IPipeBuilder<ReceiveContext> builder)
```

#### Parameters

`builder` [IPipeBuilder\<ReceiveContext\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
