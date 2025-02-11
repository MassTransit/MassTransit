---

title: RedeliveryRetryPipeSpecification<TMessage>

---

# RedeliveryRetryPipeSpecification\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class RedeliveryRetryPipeSpecification<TMessage> : ExceptionSpecification, IExceptionConfigurator, IRedeliveryConfigurator, IRetryConfigurator, IRetryObserverConnector, IPipeSpecification<ConsumeContext<TMessage>>, ISpecification
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExceptionSpecification](../../masstransit-abstractions/masstransit-configuration/exceptionspecification) → [RedeliveryRetryPipeSpecification\<TMessage\>](../masstransit-configuration/redeliveryretrypipespecification-1)<br/>
Implements [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator), [IRedeliveryConfigurator](../masstransit/iredeliveryconfigurator), [IRetryConfigurator](../masstransit/iretryconfigurator), [IRetryObserverConnector](../../masstransit-abstractions/masstransit/iretryobserverconnector), [IPipeSpecification\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **ReplaceMessageId**

```csharp
public bool ReplaceMessageId { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **RedeliveryRetryPipeSpecification(IRedeliveryPipeSpecification)**

```csharp
public RedeliveryRetryPipeSpecification(IRedeliveryPipeSpecification redeliveryPipeSpecification)
```

#### Parameters

`redeliveryPipeSpecification` [IRedeliveryPipeSpecification](../masstransit-configuration/iredeliverypipespecification)<br/>

## Methods

### **Apply(IPipeBuilder\<ConsumeContext\<TMessage\>\>)**

```csharp
public void Apply(IPipeBuilder<ConsumeContext<TMessage>> builder)
```

#### Parameters

`builder` [IPipeBuilder\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

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
