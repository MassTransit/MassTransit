---

title: RetryPipeSpecification<TContext>

---

# RetryPipeSpecification\<TContext\>

Namespace: MassTransit.Configuration

```csharp
public class RetryPipeSpecification<TContext> : ExceptionSpecification, IExceptionConfigurator, IRetryConfigurator, IRetryObserverConnector, IPipeSpecification<TContext>, ISpecification
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExceptionSpecification](../../masstransit-abstractions/masstransit-configuration/exceptionspecification) → [RetryPipeSpecification\<TContext\>](../masstransit-configuration/retrypipespecification-1)<br/>
Implements [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator), [IRetryConfigurator](../masstransit/iretryconfigurator), [IRetryObserverConnector](../../masstransit-abstractions/masstransit/iretryobserverconnector), [IPipeSpecification\<TContext\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **RetryPipeSpecification()**

```csharp
public RetryPipeSpecification()
```

## Methods

### **Apply(IPipeBuilder\<TContext\>)**

```csharp
public void Apply(IPipeBuilder<TContext> builder)
```

#### Parameters

`builder` [IPipeBuilder\<TContext\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

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

### **ConnectRetryObserver(IRetryObserver)**

```csharp
public ConnectHandle ConnectRetryObserver(IRetryObserver observer)
```

#### Parameters

`observer` [IRetryObserver](../../masstransit-abstractions/masstransit/iretryobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
