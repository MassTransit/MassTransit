---

title: ActivityConfigurationObservable

---

# ActivityConfigurationObservable

Namespace: MassTransit.Configuration

```csharp
public class ActivityConfigurationObservable : Connectable<IActivityConfigurationObserver>, IActivityConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IActivityConfigurationObserver\>](../masstransit-util/connectable-1) → [ActivityConfigurationObservable](../masstransit-configuration/activityconfigurationobservable)<br/>
Implements [IActivityConfigurationObserver](../masstransit/iactivityconfigurationobserver)

## Properties

### **Connected**

```csharp
public IActivityConfigurationObserver[] Connected { get; }
```

#### Property Value

[IActivityConfigurationObserver[]](../masstransit/iactivityconfigurationobserver)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **ActivityConfigurationObservable()**

```csharp
public ActivityConfigurationObservable()
```

## Methods

### **ActivityConfigured\<TActivity, TArguments\>(IExecuteActivityConfigurator\<TActivity, TArguments\>, Uri)**

```csharp
public void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IExecuteActivityConfigurator\<TActivity, TArguments\>](../masstransit/iexecuteactivityconfigurator-2)<br/>

`compensateAddress` Uri<br/>

### **ExecuteActivityConfigured\<TActivity, TArguments\>(IExecuteActivityConfigurator\<TActivity, TArguments\>)**

```csharp
public void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IExecuteActivityConfigurator\<TActivity, TArguments\>](../masstransit/iexecuteactivityconfigurator-2)<br/>

### **CompensateActivityConfigured\<TActivity, TLog\>(ICompensateActivityConfigurator\<TActivity, TLog\>)**

```csharp
public void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`configurator` [ICompensateActivityConfigurator\<TActivity, TLog\>](../masstransit/icompensateactivityconfigurator-2)<br/>

### **Method4()**

```csharp
public void Method4()
```

### **Method5()**

```csharp
public void Method5()
```

### **Method6()**

```csharp
public void Method6()
```
