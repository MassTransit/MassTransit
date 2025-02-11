---

title: BehaviorContextRetryConfigurator

---

# BehaviorContextRetryConfigurator

Namespace: MassTransit.Configuration

```csharp
public class BehaviorContextRetryConfigurator : ExceptionSpecification, IExceptionConfigurator, IRetryConfigurator, IRetryObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExceptionSpecification](../../masstransit-abstractions/masstransit-configuration/exceptionspecification) → [BehaviorContextRetryConfigurator](../masstransit-configuration/behaviorcontextretryconfigurator)<br/>
Implements [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator), [IRetryConfigurator](../masstransit/iretryconfigurator), [IRetryObserverConnector](../../masstransit-abstractions/masstransit/iretryobserverconnector)

## Properties

### **PolicyFactory**

```csharp
public RetryPolicyFactory PolicyFactory { get; private set; }
```

#### Property Value

[RetryPolicyFactory](../../masstransit-abstractions/masstransit-configuration/retrypolicyfactory)<br/>

## Constructors

### **BehaviorContextRetryConfigurator()**

```csharp
public BehaviorContextRetryConfigurator()
```

## Methods

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

### **GetRetryPolicy()**

```csharp
public IRetryPolicy GetRetryPolicy()
```

#### Returns

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>
