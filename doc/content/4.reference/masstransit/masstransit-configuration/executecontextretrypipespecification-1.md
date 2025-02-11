---

title: ExecuteContextRetryPipeSpecification<TArguments>

---

# ExecuteContextRetryPipeSpecification\<TArguments\>

Namespace: MassTransit.Configuration

```csharp
public class ExecuteContextRetryPipeSpecification<TArguments> : ExceptionSpecification, IExceptionConfigurator, IRetryConfigurator, IRetryObserverConnector, IPipeSpecification<ExecuteContext<TArguments>>, ISpecification
```

#### Type Parameters

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExceptionSpecification](../../masstransit-abstractions/masstransit-configuration/exceptionspecification) → [ExecuteContextRetryPipeSpecification\<TArguments\>](../masstransit-configuration/executecontextretrypipespecification-1)<br/>
Implements [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator), [IRetryConfigurator](../masstransit/iretryconfigurator), [IRetryObserverConnector](../../masstransit-abstractions/masstransit/iretryobserverconnector), [IPipeSpecification\<ExecuteContext\<TArguments\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **ExecuteContextRetryPipeSpecification(CancellationToken)**

```csharp
public ExecuteContextRetryPipeSpecification(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

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

### **SetRetryPolicy(RetryPolicyFactory)**

```csharp
public void SetRetryPolicy(RetryPolicyFactory factory)
```

#### Parameters

`factory` [RetryPolicyFactory](../../masstransit-abstractions/masstransit-configuration/retrypolicyfactory)<br/>
