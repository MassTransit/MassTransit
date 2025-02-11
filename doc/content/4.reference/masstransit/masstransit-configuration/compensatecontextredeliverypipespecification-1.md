---

title: CompensateContextRedeliveryPipeSpecification<TLog>

---

# CompensateContextRedeliveryPipeSpecification\<TLog\>

Namespace: MassTransit.Configuration

```csharp
public class CompensateContextRedeliveryPipeSpecification<TLog> : ExceptionSpecification, IExceptionConfigurator, IRedeliveryConfigurator, IRetryConfigurator, IRetryObserverConnector, IPipeSpecification<CompensateContext<TLog>>, ISpecification
```

#### Type Parameters

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExceptionSpecification](../../masstransit-abstractions/masstransit-configuration/exceptionspecification) → [CompensateContextRedeliveryPipeSpecification\<TLog\>](../masstransit-configuration/compensatecontextredeliverypipespecification-1)<br/>
Implements [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator), [IRedeliveryConfigurator](../masstransit/iredeliveryconfigurator), [IRetryConfigurator](../masstransit/iretryconfigurator), [IRetryObserverConnector](../../masstransit-abstractions/masstransit/iretryobserverconnector), [IPipeSpecification\<CompensateContext\<TLog\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **ReplaceMessageId**

```csharp
public bool ReplaceMessageId { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **CompensateContextRedeliveryPipeSpecification()**

```csharp
public CompensateContextRedeliveryPipeSpecification()
```

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
