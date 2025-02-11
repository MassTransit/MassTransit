---

title: CompensateActivityEndpointDefinition<TActivity, TLog>

---

# CompensateActivityEndpointDefinition\<TActivity, TLog\>

Namespace: MassTransit.Configuration

```csharp
public class CompensateActivityEndpointDefinition<TActivity, TLog> : SettingsEndpointDefinition<ICompensateActivity<TLog>>, IEndpointDefinition<ICompensateActivity<TLog>>, IEndpointDefinition
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SettingsEndpointDefinition\<ICompensateActivity\<TLog\>\>](../masstransit-configuration/settingsendpointdefinition-1) → [CompensateActivityEndpointDefinition\<TActivity, TLog\>](../masstransit-configuration/compensateactivityendpointdefinition-2)<br/>
Implements [IEndpointDefinition\<ICompensateActivity\<TLog\>\>](../masstransit/iendpointdefinition-1), [IEndpointDefinition](../masstransit/iendpointdefinition)

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

### **CompensateActivityEndpointDefinition(IEndpointSettings\<IEndpointDefinition\<ICompensateActivity\<TLog\>\>\>)**

```csharp
public CompensateActivityEndpointDefinition(IEndpointSettings<IEndpointDefinition<ICompensateActivity<TLog>>> settings)
```

#### Parameters

`settings` [IEndpointSettings\<IEndpointDefinition\<ICompensateActivity\<TLog\>\>\>](../masstransit/iendpointsettings-1)<br/>

## Methods

### **FormatEndpointName(IEndpointNameFormatter)**

```csharp
protected string FormatEndpointName(IEndpointNameFormatter formatter)
```

#### Parameters

`formatter` [IEndpointNameFormatter](../masstransit/iendpointnameformatter)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
