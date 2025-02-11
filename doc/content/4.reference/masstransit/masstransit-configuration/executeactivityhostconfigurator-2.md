---

title: ExecuteActivityHostConfigurator<TActivity, TArguments>

---

# ExecuteActivityHostConfigurator\<TActivity, TArguments\>

Namespace: MassTransit.Configuration

```csharp
public class ExecuteActivityHostConfigurator<TActivity, TArguments> : IExecuteActivityConfigurator<TActivity, TArguments>, IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>>, IActivityObserverConnector, IConsumeConfigurator, IReceiveEndpointSpecification, ISpecification
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExecuteActivityHostConfigurator\<TActivity, TArguments\>](../masstransit-configuration/executeactivityhostconfigurator-2)<br/>
Implements [IExecuteActivityConfigurator\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityconfigurator-2), [IPipeConfigurator\<ExecuteActivityContext\<TActivity, TArguments\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IActivityObserverConnector](../../masstransit-abstractions/masstransit/iactivityobserverconnector), [IConsumeConfigurator](../../masstransit-abstractions/masstransit/iconsumeconfigurator), [IReceiveEndpointSpecification](../../masstransit-abstractions/masstransit/ireceiveendpointspecification), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **ExecuteActivityHostConfigurator(IExecuteActivityFactory\<TActivity, TArguments\>, IActivityConfigurationObserver)**

```csharp
public ExecuteActivityHostConfigurator(IExecuteActivityFactory<TActivity, TArguments> activityFactory, IActivityConfigurationObserver observer)
```

#### Parameters

`activityFactory` [IExecuteActivityFactory\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityfactory-2)<br/>

`observer` [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver)<br/>

### **ExecuteActivityHostConfigurator(IExecuteActivityFactory\<TActivity, TArguments\>, Uri, IActivityConfigurationObserver)**

```csharp
public ExecuteActivityHostConfigurator(IExecuteActivityFactory<TActivity, TArguments> activityFactory, Uri compensateAddress, IActivityConfigurationObserver observer)
```

#### Parameters

`activityFactory` [IExecuteActivityFactory\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityfactory-2)<br/>

`compensateAddress` Uri<br/>

`observer` [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver)<br/>

## Methods

### **AddPipeSpecification(IPipeSpecification\<ExecuteActivityContext\<TActivity, TArguments\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<ExecuteActivityContext<TActivity, TArguments>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ExecuteActivityContext\<TActivity, TArguments\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **Arguments(Action\<IExecuteArgumentsConfigurator\<TArguments\>\>)**

```csharp
public void Arguments(Action<IExecuteArgumentsConfigurator<TArguments>> configure)
```

#### Parameters

`configure` [Action\<IExecuteArgumentsConfigurator\<TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ActivityArguments(Action\<IExecuteActivityArgumentsConfigurator\<TArguments\>\>)**

```csharp
public void ActivityArguments(Action<IExecuteActivityArgumentsConfigurator<TArguments>> configure)
```

#### Parameters

`configure` [Action\<IExecuteActivityArgumentsConfigurator\<TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

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
