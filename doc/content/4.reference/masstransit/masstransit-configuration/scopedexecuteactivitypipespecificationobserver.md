---

title: ScopedExecuteActivityPipeSpecificationObserver

---

# ScopedExecuteActivityPipeSpecificationObserver

Namespace: MassTransit.Configuration

```csharp
public class ScopedExecuteActivityPipeSpecificationObserver : IActivityConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopedExecuteActivityPipeSpecificationObserver](../masstransit-configuration/scopedexecuteactivitypipespecificationobserver)<br/>
Implements [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver)

## Constructors

### **ScopedExecuteActivityPipeSpecificationObserver(Type, IRegistrationContext, CompositeFilter\<Type\>)**

```csharp
public ScopedExecuteActivityPipeSpecificationObserver(Type filterType, IRegistrationContext context, CompositeFilter<Type> messageTypeFilter)
```

#### Parameters

`filterType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`messageTypeFilter` [CompositeFilter\<Type\>](../masstransit-configuration/compositefilter-1)<br/>

## Methods

### **ActivityConfigured\<TActivity, TArguments\>(IExecuteActivityConfigurator\<TActivity, TArguments\>, Uri)**

```csharp
public void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IExecuteActivityConfigurator\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityconfigurator-2)<br/>

`compensateAddress` Uri<br/>

### **ExecuteActivityConfigured\<TActivity, TArguments\>(IExecuteActivityConfigurator\<TActivity, TArguments\>)**

```csharp
public void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IExecuteActivityConfigurator\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityconfigurator-2)<br/>

### **CompensateActivityConfigured\<TActivity, TLog\>(ICompensateActivityConfigurator\<TActivity, TLog\>)**

```csharp
public void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`configurator` [ICompensateActivityConfigurator\<TActivity, TLog\>](../../masstransit-abstractions/masstransit/icompensateactivityconfigurator-2)<br/>
