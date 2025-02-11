---

title: ExecuteContextRedeliveryPipeSpecification<TArguments>

---

# ExecuteContextRedeliveryPipeSpecification\<TArguments\>

Namespace: MassTransit.Configuration

```csharp
public class ExecuteContextRedeliveryPipeSpecification<TArguments> : ExceptionSpecification, IExceptionConfigurator, IRedeliveryConfigurator, IRetryConfigurator, IRetryObserverConnector, IPipeSpecification<ExecuteContext<TArguments>>, ISpecification
```

#### Type Parameters

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExceptionSpecification](../../masstransit-abstractions/masstransit-configuration/exceptionspecification) → [ExecuteContextRedeliveryPipeSpecification\<TArguments\>](../masstransit-configuration/executecontextredeliverypipespecification-1)<br/>
Implements [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator), [IRedeliveryConfigurator](../masstransit/iredeliveryconfigurator), [IRetryConfigurator](../masstransit/iretryconfigurator), [IRetryObserverConnector](../../masstransit-abstractions/masstransit/iretryobserverconnector), [IPipeSpecification\<ExecuteContext\<TArguments\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **ReplaceMessageId**

```csharp
public bool ReplaceMessageId { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **ExecuteContextRedeliveryPipeSpecification()**

```csharp
public ExecuteContextRedeliveryPipeSpecification()
```

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
