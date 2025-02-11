---

title: DefaultExecuteActivityDefinition<TActivity, TArguments>

---

# DefaultExecuteActivityDefinition\<TActivity, TArguments\>

Namespace: MassTransit.Configuration

```csharp
public class DefaultExecuteActivityDefinition<TActivity, TArguments> : ExecuteActivityDefinition<TActivity, TArguments>, IExecuteActivityDefinition<TActivity, TArguments>, IExecuteActivityDefinition, IDefinition
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExecuteActivityDefinition\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/executeactivitydefinition-2) → [DefaultExecuteActivityDefinition\<TActivity, TArguments\>](../masstransit-configuration/defaultexecuteactivitydefinition-2)<br/>
Implements [IExecuteActivityDefinition\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivitydefinition-2), [IExecuteActivityDefinition](../../masstransit-abstractions/masstransit/iexecuteactivitydefinition), [IDefinition](../../masstransit-abstractions/masstransit/idefinition)

## Properties

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

### **DefaultExecuteActivityDefinition()**

```csharp
public DefaultExecuteActivityDefinition()
```
