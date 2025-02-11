---

title: IExecuteActivityDefinition<TActivity, TArguments>

---

# IExecuteActivityDefinition\<TActivity, TArguments\>

Namespace: MassTransit

```csharp
public interface IExecuteActivityDefinition<TActivity, TArguments> : IExecuteActivityDefinition, IDefinition
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Implements [IExecuteActivityDefinition](../masstransit/iexecuteactivitydefinition), [IDefinition](../masstransit/idefinition)

## Properties

### **ExecuteEndpointDefinition**

Sets the endpoint definition, if available

```csharp
public abstract IEndpointDefinition<IExecuteActivity<TArguments>> ExecuteEndpointDefinition { set; }
```

#### Property Value

[IEndpointDefinition\<IExecuteActivity\<TArguments\>\>](../masstransit/iendpointdefinition-1)<br/>

## Methods

### **Configure(IReceiveEndpointConfigurator, IExecuteActivityConfigurator\<TActivity, TArguments\>, IRegistrationContext)**

Configure the execute activity

```csharp
void Configure(IReceiveEndpointConfigurator endpointConfigurator, IExecuteActivityConfigurator<TActivity, TArguments> executeActivityConfigurator, IRegistrationContext context)
```

#### Parameters

`endpointConfigurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>

`executeActivityConfigurator` [IExecuteActivityConfigurator\<TActivity, TArguments\>](../masstransit/iexecuteactivityconfigurator-2)<br/>

`context` [IRegistrationContext](../masstransit/iregistrationcontext)<br/>
