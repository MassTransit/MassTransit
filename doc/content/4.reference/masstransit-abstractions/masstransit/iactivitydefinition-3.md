---

title: IActivityDefinition<TActivity, TArguments, TLog>

---

# IActivityDefinition\<TActivity, TArguments, TLog\>

Namespace: MassTransit

```csharp
public interface IActivityDefinition<TActivity, TArguments, TLog> : IActivityDefinition, IExecuteActivityDefinition, IDefinition, IExecuteActivityDefinition<TActivity, TArguments>
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

`TLog`<br/>

Implements [IActivityDefinition](../masstransit/iactivitydefinition), [IExecuteActivityDefinition](../masstransit/iexecuteactivitydefinition), [IDefinition](../masstransit/idefinition), [IExecuteActivityDefinition\<TActivity, TArguments\>](../masstransit/iexecuteactivitydefinition-2)

## Properties

### **CompensateEndpointDefinition**

Sets the endpoint definition, if available

```csharp
public abstract IEndpointDefinition<ICompensateActivity<TLog>> CompensateEndpointDefinition { set; }
```

#### Property Value

[IEndpointDefinition\<ICompensateActivity\<TLog\>\>](../masstransit/iendpointdefinition-1)<br/>

## Methods

### **Configure(IReceiveEndpointConfigurator, ICompensateActivityConfigurator\<TActivity, TLog\>, IRegistrationContext)**

Configure the compensate activity

```csharp
void Configure(IReceiveEndpointConfigurator endpointConfigurator, ICompensateActivityConfigurator<TActivity, TLog> compensateActivityConfigurator, IRegistrationContext context)
```

#### Parameters

`endpointConfigurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>

`compensateActivityConfigurator` [ICompensateActivityConfigurator\<TActivity, TLog\>](../masstransit/icompensateactivityconfigurator-2)<br/>

`context` [IRegistrationContext](../masstransit/iregistrationcontext)<br/>
