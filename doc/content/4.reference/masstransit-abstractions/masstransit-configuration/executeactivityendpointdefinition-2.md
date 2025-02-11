---

title: ExecuteActivityEndpointDefinition<TActivity, TArguments>

---

# ExecuteActivityEndpointDefinition\<TActivity, TArguments\>

Namespace: MassTransit.Configuration

```csharp
public class ExecuteActivityEndpointDefinition<TActivity, TArguments> : SettingsEndpointDefinition<IExecuteActivity<TArguments>>, IEndpointDefinition<IExecuteActivity<TArguments>>, IEndpointDefinition
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SettingsEndpointDefinition\<IExecuteActivity\<TArguments\>\>](../masstransit-configuration/settingsendpointdefinition-1) → [ExecuteActivityEndpointDefinition\<TActivity, TArguments\>](../masstransit-configuration/executeactivityendpointdefinition-2)<br/>
Implements [IEndpointDefinition\<IExecuteActivity\<TArguments\>\>](../masstransit/iendpointdefinition-1), [IEndpointDefinition](../masstransit/iendpointdefinition)

## Properties

### **IsTemporary**

```csharp
public bool IsTemporary { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **PrefetchCount**

```csharp
public Nullable<int> PrefetchCount { get; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { get; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConfigureConsumeTopology**

```csharp
public bool ConfigureConsumeTopology { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **ExecuteActivityEndpointDefinition(IEndpointSettings\<IEndpointDefinition\<IExecuteActivity\<TArguments\>\>\>)**

```csharp
public ExecuteActivityEndpointDefinition(IEndpointSettings<IEndpointDefinition<IExecuteActivity<TArguments>>> settings)
```

#### Parameters

`settings` [IEndpointSettings\<IEndpointDefinition\<IExecuteActivity\<TArguments\>\>\>](../masstransit/iendpointsettings-1)<br/>

## Methods

### **FormatEndpointName(IEndpointNameFormatter)**

```csharp
protected string FormatEndpointName(IEndpointNameFormatter formatter)
```

#### Parameters

`formatter` [IEndpointNameFormatter](../masstransit/iendpointnameformatter)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
