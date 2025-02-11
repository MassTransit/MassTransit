---

title: IExecuteActivityConfigurator<TActivity, TArguments>

---

# IExecuteActivityConfigurator\<TActivity, TArguments\>

Namespace: MassTransit

Configure the execution of the activity and arguments with some tasty middleware.

```csharp
public interface IExecuteActivityConfigurator<TActivity, TArguments> : IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>>, IActivityObserverConnector, IConsumeConfigurator
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Implements [IPipeConfigurator\<ExecuteActivityContext\<TActivity, TArguments\>\>](../masstransit/ipipeconfigurator-1), [IActivityObserverConnector](../masstransit/iactivityobserverconnector), [IConsumeConfigurator](../masstransit/iconsumeconfigurator)

## Properties

### **ConcurrentMessageLimit**

```csharp
public abstract Nullable<int> ConcurrentMessageLimit { set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Methods

### **Arguments(Action\<IExecuteArgumentsConfigurator\<TArguments\>\>)**

Configure the pipeline prior to the activity factory

```csharp
void Arguments(Action<IExecuteArgumentsConfigurator<TArguments>> configure)
```

#### Parameters

`configure` [Action\<IExecuteArgumentsConfigurator\<TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ActivityArguments(Action\<IExecuteActivityArgumentsConfigurator\<TArguments\>\>)**

Configure the arguments separate from the activity

```csharp
void ActivityArguments(Action<IExecuteActivityArgumentsConfigurator<TArguments>> configure)
```

#### Parameters

`configure` [Action\<IExecuteActivityArgumentsConfigurator\<TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **RoutingSlip(Action\<IRoutingSlipConfigurator\>)**

Configure the routing slip pipe

```csharp
void RoutingSlip(Action<IRoutingSlipConfigurator> configure)
```

#### Parameters

`configure` [Action\<IRoutingSlipConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
