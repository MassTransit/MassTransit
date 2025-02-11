---

title: CompensateContextRetryPipeSpecification<TLog>

---

# CompensateContextRetryPipeSpecification\<TLog\>

Namespace: MassTransit.Configuration

```csharp
public class CompensateContextRetryPipeSpecification<TLog> : ExceptionSpecification, IExceptionConfigurator, IRetryConfigurator, IRetryObserverConnector, IPipeSpecification<CompensateContext<TLog>>, ISpecification
```

#### Type Parameters

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExceptionSpecification](../../masstransit-abstractions/masstransit-configuration/exceptionspecification) → [CompensateContextRetryPipeSpecification\<TLog\>](../masstransit-configuration/compensatecontextretrypipespecification-1)<br/>
Implements [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator), [IRetryConfigurator](../masstransit/iretryconfigurator), [IRetryObserverConnector](../../masstransit-abstractions/masstransit/iretryobserverconnector), [IPipeSpecification\<CompensateContext\<TLog\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **CompensateContextRetryPipeSpecification(CancellationToken)**

```csharp
public CompensateContextRetryPipeSpecification(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **Apply(IPipeBuilder\<CompensateContext\<TLog\>\>)**

```csharp
public void Apply(IPipeBuilder<CompensateContext<TLog>> builder)
```

#### Parameters

`builder` [IPipeBuilder\<CompensateContext\<TLog\>\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

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
