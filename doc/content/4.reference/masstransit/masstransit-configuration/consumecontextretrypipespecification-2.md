---

title: ConsumeContextRetryPipeSpecification<TFilter, TContext>

---

# ConsumeContextRetryPipeSpecification\<TFilter, TContext\>

Namespace: MassTransit.Configuration

```csharp
public class ConsumeContextRetryPipeSpecification<TFilter, TContext> : ExceptionSpecification, IExceptionConfigurator, IRetryConfigurator, IRetryObserverConnector, IPipeSpecification<TFilter>, ISpecification
```

#### Type Parameters

`TFilter`<br/>

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExceptionSpecification](../../masstransit-abstractions/masstransit-configuration/exceptionspecification) → [ConsumeContextRetryPipeSpecification\<TFilter, TContext\>](../masstransit-configuration/consumecontextretrypipespecification-2)<br/>
Implements [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator), [IRetryConfigurator](../masstransit/iretryconfigurator), [IRetryObserverConnector](../../masstransit-abstractions/masstransit/iretryobserverconnector), [IPipeSpecification\<TFilter\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **ConsumeContextRetryPipeSpecification(Func\<TFilter, IRetryPolicy, RetryContext, TContext\>, CancellationToken)**

```csharp
public ConsumeContextRetryPipeSpecification(Func<TFilter, IRetryPolicy, RetryContext, TContext> contextFactory, CancellationToken cancellationToken)
```

#### Parameters

`contextFactory` [Func\<TFilter, IRetryPolicy, RetryContext, TContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **Apply(IPipeBuilder\<TFilter\>)**

```csharp
public void Apply(IPipeBuilder<TFilter> builder)
```

#### Parameters

`builder` [IPipeBuilder\<TFilter\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

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
