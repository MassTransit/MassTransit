---

title: SagaEndpointDefinition<TSaga>

---

# SagaEndpointDefinition\<TSaga\>

Namespace: MassTransit.Configuration

```csharp
public class SagaEndpointDefinition<TSaga> : SettingsEndpointDefinition<TSaga>, IEndpointDefinition<TSaga>, IEndpointDefinition
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SettingsEndpointDefinition\<TSaga\>](../masstransit-configuration/settingsendpointdefinition-1) → [SagaEndpointDefinition\<TSaga\>](../masstransit-configuration/sagaendpointdefinition-1)<br/>
Implements [IEndpointDefinition\<TSaga\>](../masstransit/iendpointdefinition-1), [IEndpointDefinition](../masstransit/iendpointdefinition)

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

### **SagaEndpointDefinition(IEndpointSettings\<IEndpointDefinition\<TSaga\>\>)**

```csharp
public SagaEndpointDefinition(IEndpointSettings<IEndpointDefinition<TSaga>> settings)
```

#### Parameters

`settings` [IEndpointSettings\<IEndpointDefinition\<TSaga\>\>](../masstransit/iendpointsettings-1)<br/>

## Methods

### **FormatEndpointName(IEndpointNameFormatter)**

```csharp
protected string FormatEndpointName(IEndpointNameFormatter formatter)
```

#### Parameters

`formatter` [IEndpointNameFormatter](../masstransit/iendpointnameformatter)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
