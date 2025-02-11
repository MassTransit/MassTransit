---

title: ExecuteActivityDefinition<TActivity, TArguments>

---

# ExecuteActivityDefinition\<TActivity, TArguments\>

Namespace: MassTransit

```csharp
public class ExecuteActivityDefinition<TActivity, TArguments> : IExecuteActivityDefinition<TActivity, TArguments>, IExecuteActivityDefinition, IDefinition
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExecuteActivityDefinition\<TActivity, TArguments\>](../masstransit/executeactivitydefinition-2)<br/>
Implements [IExecuteActivityDefinition\<TActivity, TArguments\>](../masstransit/iexecuteactivitydefinition-2), [IExecuteActivityDefinition](../masstransit/iexecuteactivitydefinition), [IDefinition](../masstransit/idefinition)

## Properties

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

## Methods

### **ExecuteEndpoint(Action\<IEndpointRegistrationConfigurator\>)**

Configure the execute endpoint

```csharp
protected void ExecuteEndpoint(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConfigureExecuteActivity(IReceiveEndpointConfigurator, IExecuteActivityConfigurator\<TActivity, TArguments\>)**

#### Caution

Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

Called when the compensate activity is being configured on the endpoint.

```csharp
protected void ConfigureExecuteActivity(IReceiveEndpointConfigurator endpointConfigurator, IExecuteActivityConfigurator<TActivity, TArguments> executeActivityConfigurator)
```

#### Parameters

`endpointConfigurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>
The receive endpoint configurator for the consumer

`executeActivityConfigurator` [IExecuteActivityConfigurator\<TActivity, TArguments\>](../masstransit/iexecuteactivityconfigurator-2)<br/>

### **ConfigureExecuteActivity(IReceiveEndpointConfigurator, IExecuteActivityConfigurator\<TActivity, TArguments\>, IRegistrationContext)**

Called when the compensate activity is being configured on the endpoint.

```csharp
protected void ConfigureExecuteActivity(IReceiveEndpointConfigurator endpointConfigurator, IExecuteActivityConfigurator<TActivity, TArguments> executeActivityConfigurator, IRegistrationContext context)
```

#### Parameters

`endpointConfigurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>
The receive endpoint configurator for the consumer

`executeActivityConfigurator` [IExecuteActivityConfigurator\<TActivity, TArguments\>](../masstransit/iexecuteactivityconfigurator-2)<br/>

`context` [IRegistrationContext](../masstransit/iregistrationcontext)<br/>
