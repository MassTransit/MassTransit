---

title: ConsumeContextRetryPipeSpecification

---

# ConsumeContextRetryPipeSpecification

Namespace: MassTransit.Configuration

```csharp
public class ConsumeContextRetryPipeSpecification : ExceptionSpecification, IExceptionConfigurator, IRetryConfigurator, IRetryObserverConnector, IPipeSpecification<ConsumeContext>, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExceptionSpecification](../../masstransit-abstractions/masstransit-configuration/exceptionspecification) → [ConsumeContextRetryPipeSpecification](../masstransit-configuration/consumecontextretrypipespecification)<br/>
Implements [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator), [IRetryConfigurator](../masstransit/iretryconfigurator), [IRetryObserverConnector](../../masstransit-abstractions/masstransit/iretryobserverconnector), [IPipeSpecification\<ConsumeContext\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **ConsumeContextRetryPipeSpecification(CancellationToken)**

```csharp
public ConsumeContextRetryPipeSpecification(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

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

### **SetRetryPolicy(RetryPolicyFactory)**

```csharp
public void SetRetryPolicy(RetryPolicyFactory factory)
```

#### Parameters

`factory` [RetryPolicyFactory](../../masstransit-abstractions/masstransit-configuration/retrypolicyfactory)<br/>
