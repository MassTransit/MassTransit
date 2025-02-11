---

title: MissingInstanceRedeliveryConfigurator<TSaga, TMessage>

---

# MissingInstanceRedeliveryConfigurator\<TSaga, TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class MissingInstanceRedeliveryConfigurator<TSaga, TMessage> : ExceptionSpecification, IExceptionConfigurator, IMissingInstanceRedeliveryConfigurator<TSaga, TMessage>, IMissingInstanceRedeliveryConfigurator, IRedeliveryConfigurator, IRetryConfigurator, IRetryObserverConnector, ISpecification
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExceptionSpecification](../../masstransit-abstractions/masstransit-configuration/exceptionspecification) → [MissingInstanceRedeliveryConfigurator\<TSaga, TMessage\>](../masstransit-configuration/missinginstanceredeliveryconfigurator-2)<br/>
Implements [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator), [IMissingInstanceRedeliveryConfigurator\<TSaga, TMessage\>](../masstransit/imissinginstanceredeliveryconfigurator-2), [IMissingInstanceRedeliveryConfigurator](../masstransit/imissinginstanceredeliveryconfigurator), [IRedeliveryConfigurator](../masstransit/iredeliveryconfigurator), [IRetryConfigurator](../masstransit/iretryconfigurator), [IRetryObserverConnector](../../masstransit-abstractions/masstransit/iretryobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **ReplaceMessageId**

```csharp
public bool ReplaceMessageId { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **UseMessageScheduler**

```csharp
public bool UseMessageScheduler { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **MissingInstanceRedeliveryConfigurator(IMissingInstanceConfigurator\<TSaga, TMessage\>)**

```csharp
public MissingInstanceRedeliveryConfigurator(IMissingInstanceConfigurator<TSaga, TMessage> configurator)
```

#### Parameters

`configurator` [IMissingInstanceConfigurator\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/imissinginstanceconfigurator-2)<br/>

## Methods

### **SetRetryPolicy(RetryPolicyFactory)**

```csharp
public void SetRetryPolicy(RetryPolicyFactory factory)
```

#### Parameters

`factory` [RetryPolicyFactory](../../masstransit-abstractions/masstransit-configuration/retrypolicyfactory)<br/>

### **OnRedeliveryLimitReached(Func\<IMissingInstanceConfigurator\<TSaga, TMessage\>, IPipe\<ConsumeContext\<TMessage\>\>\>)**

```csharp
public void OnRedeliveryLimitReached(Func<IMissingInstanceConfigurator<TSaga, TMessage>, IPipe<ConsumeContext<TMessage>>> configure)
```

#### Parameters

`configure` [Func\<IMissingInstanceConfigurator\<TSaga, TMessage\>, IPipe\<ConsumeContext\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **ConnectRetryObserver(IRetryObserver)**

```csharp
public ConnectHandle ConnectRetryObserver(IRetryObserver observer)
```

#### Parameters

`observer` [IRetryObserver](../../masstransit-abstractions/masstransit/iretryobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Build()**

```csharp
public IPipe<ConsumeContext<TMessage>> Build()
```

#### Returns

[IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>
