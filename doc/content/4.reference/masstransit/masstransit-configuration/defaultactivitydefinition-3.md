---

title: DefaultActivityDefinition<TActivity, TArguments, TLog>

---

# DefaultActivityDefinition\<TActivity, TArguments, TLog\>

Namespace: MassTransit.Configuration

```csharp
public class DefaultActivityDefinition<TActivity, TArguments, TLog> : ActivityDefinition<TActivity, TArguments, TLog>, IExecuteActivityDefinition<TActivity, TArguments>, IExecuteActivityDefinition, IDefinition, IActivityDefinition<TActivity, TArguments, TLog>, IActivityDefinition
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExecuteActivityDefinition\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/executeactivitydefinition-2) → [ActivityDefinition\<TActivity, TArguments, TLog\>](../../masstransit-abstractions/masstransit/activitydefinition-3) → [DefaultActivityDefinition\<TActivity, TArguments, TLog\>](../masstransit-configuration/defaultactivitydefinition-3)<br/>
Implements [IExecuteActivityDefinition\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivitydefinition-2), [IExecuteActivityDefinition](../../masstransit-abstractions/masstransit/iexecuteactivitydefinition), [IDefinition](../../masstransit-abstractions/masstransit/idefinition), [IActivityDefinition\<TActivity, TArguments, TLog\>](../../masstransit-abstractions/masstransit/iactivitydefinition-3), [IActivityDefinition](../../masstransit-abstractions/masstransit/iactivitydefinition)

## Properties

### **CompensateEndpointDefinition**

```csharp
public IEndpointDefinition<ICompensateActivity<TLog>> CompensateEndpointDefinition { get; set; }
```

#### Property Value

[IEndpointDefinition\<ICompensateActivity\<TLog\>\>](../../masstransit-abstractions/masstransit/iendpointdefinition-1)<br/>

### **ExecuteEndpointDefinition**

```csharp
public IEndpointDefinition<IExecuteActivity<TArguments>> ExecuteEndpointDefinition { get; set; }
```

#### Property Value

[IEndpointDefinition\<IExecuteActivity\<TArguments\>\>](../../masstransit-abstractions/masstransit/iendpointdefinition-1)<br/>

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { get; protected set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **DefaultActivityDefinition()**

```csharp
public DefaultActivityDefinition()
```
