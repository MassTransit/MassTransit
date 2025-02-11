---

title: CompensateActivityHostConfigurator<TActivity, TLog>

---

# CompensateActivityHostConfigurator\<TActivity, TLog\>

Namespace: MassTransit.Configuration

```csharp
public class CompensateActivityHostConfigurator<TActivity, TLog> : ICompensateActivityConfigurator<TActivity, TLog>, IPipeConfigurator<CompensateActivityContext<TActivity, TLog>>, IActivityObserverConnector, IConsumeConfigurator, IReceiveEndpointSpecification, ISpecification
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CompensateActivityHostConfigurator\<TActivity, TLog\>](../masstransit-configuration/compensateactivityhostconfigurator-2)<br/>
Implements [ICompensateActivityConfigurator\<TActivity, TLog\>](../../masstransit-abstractions/masstransit/icompensateactivityconfigurator-2), [IPipeConfigurator\<CompensateActivityContext\<TActivity, TLog\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IActivityObserverConnector](../../masstransit-abstractions/masstransit/iactivityobserverconnector), [IConsumeConfigurator](../../masstransit-abstractions/masstransit/iconsumeconfigurator), [IReceiveEndpointSpecification](../../masstransit-abstractions/masstransit/ireceiveendpointspecification), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **CompensateActivityHostConfigurator(ICompensateActivityFactory\<TActivity, TLog\>, IActivityConfigurationObserver)**

```csharp
public CompensateActivityHostConfigurator(ICompensateActivityFactory<TActivity, TLog> activityFactory, IActivityConfigurationObserver observer)
```

#### Parameters

`activityFactory` [ICompensateActivityFactory\<TActivity, TLog\>](../../masstransit-abstractions/masstransit/icompensateactivityfactory-2)<br/>

`observer` [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver)<br/>

## Methods

### **AddPipeSpecification(IPipeSpecification\<CompensateActivityContext\<TActivity, TLog\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<CompensateActivityContext<TActivity, TLog>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<CompensateActivityContext\<TActivity, TLog\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **Log(Action\<ICompensateLogConfigurator\<TLog\>\>)**

```csharp
public void Log(Action<ICompensateLogConfigurator<TLog>> configure)
```

#### Parameters

`configure` [Action\<ICompensateLogConfigurator\<TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ActivityLog(Action\<ICompensateActivityLogConfigurator\<TLog\>\>)**

```csharp
public void ActivityLog(Action<ICompensateActivityLogConfigurator<TLog>> configure)
```

#### Parameters

`configure` [Action\<ICompensateActivityLogConfigurator\<TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **RoutingSlip(Action\<IRoutingSlipConfigurator\>)**

```csharp
public void RoutingSlip(Action<IRoutingSlipConfigurator> configure)
```

#### Parameters

`configure` [Action\<IRoutingSlipConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConnectActivityObserver(IActivityObserver)**

```csharp
public ConnectHandle ConnectActivityObserver(IActivityObserver observer)
```

#### Parameters

`observer` [IActivityObserver](../../masstransit-abstractions/masstransit/iactivityobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Configure(IReceiveEndpointBuilder)**

```csharp
public void Configure(IReceiveEndpointBuilder builder)
```

#### Parameters

`builder` [IReceiveEndpointBuilder](../../masstransit-abstractions/masstransit-configuration/ireceiveendpointbuilder)<br/>
