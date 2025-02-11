---

title: ActivityDefinition<TActivity, TArguments, TLog>

---

# ActivityDefinition\<TActivity, TArguments, TLog\>

Namespace: MassTransit

```csharp
public class ActivityDefinition<TActivity, TArguments, TLog> : ExecuteActivityDefinition<TActivity, TArguments>, IExecuteActivityDefinition<TActivity, TArguments>, IExecuteActivityDefinition, IDefinition, IActivityDefinition<TActivity, TArguments, TLog>, IActivityDefinition
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExecuteActivityDefinition\<TActivity, TArguments\>](../masstransit/executeactivitydefinition-2) → [ActivityDefinition\<TActivity, TArguments, TLog\>](../masstransit/activitydefinition-3)<br/>
Implements [IExecuteActivityDefinition\<TActivity, TArguments\>](../masstransit/iexecuteactivitydefinition-2), [IExecuteActivityDefinition](../masstransit/iexecuteactivitydefinition), [IDefinition](../masstransit/idefinition), [IActivityDefinition\<TActivity, TArguments, TLog\>](../masstransit/iactivitydefinition-3), [IActivityDefinition](../masstransit/iactivitydefinition)

## Properties

### **CompensateEndpointDefinition**

```csharp
public IEndpointDefinition<ICompensateActivity<TLog>> CompensateEndpointDefinition { get; set; }
```

#### Property Value

[IEndpointDefinition\<ICompensateActivity\<TLog\>\>](../masstransit/iendpointdefinition-1)<br/>

### **ExecuteEndpointDefinition**

```csharp
public IEndpointDefinition<IExecuteActivity<TArguments>> ExecuteEndpointDefinition { get; set; }
```

#### Property Value

[IEndpointDefinition\<IExecuteActivity\<TArguments\>\>](../masstransit/iendpointdefinition-1)<br/>

### **ConcurrentMessageLimit**

Specify a concurrency limit, which is applied to the entire consumer, saga, or activity, regardless of message type.

```csharp
public Nullable<int> ConcurrentMessageLimit { get; protected set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **ActivityDefinition()**

```csharp
public ActivityDefinition()
```

## Methods

### **CompensateEndpoint(Action\<IEndpointRegistrationConfigurator\>)**

Configure the compensate endpoint

```csharp
protected void CompensateEndpoint(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConfigureCompensateActivity(IReceiveEndpointConfigurator, ICompensateActivityConfigurator\<TActivity, TLog\>)**

#### Caution

Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

Called when the compensate activity is being configured on the endpoint.

```csharp
protected void ConfigureCompensateActivity(IReceiveEndpointConfigurator endpointConfigurator, ICompensateActivityConfigurator<TActivity, TLog> compensateActivityConfigurator)
```

#### Parameters

`endpointConfigurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>
The receive endpoint configurator for the consumer

`compensateActivityConfigurator` [ICompensateActivityConfigurator\<TActivity, TLog\>](../masstransit/icompensateactivityconfigurator-2)<br/>

### **ConfigureCompensateActivity(IReceiveEndpointConfigurator, ICompensateActivityConfigurator\<TActivity, TLog\>, IRegistrationContext)**

Called when the compensate activity is being configured on the endpoint.

```csharp
protected void ConfigureCompensateActivity(IReceiveEndpointConfigurator endpointConfigurator, ICompensateActivityConfigurator<TActivity, TLog> compensateActivityConfigurator, IRegistrationContext context)
```

#### Parameters

`endpointConfigurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>
The receive endpoint configurator for the consumer

`compensateActivityConfigurator` [ICompensateActivityConfigurator\<TActivity, TLog\>](../masstransit/icompensateactivityconfigurator-2)<br/>

`context` [IRegistrationContext](../masstransit/iregistrationcontext)<br/>
