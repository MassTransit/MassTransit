---

title: ConsumerEndpointDefinition<TConsumer>

---

# ConsumerEndpointDefinition\<TConsumer\>

Namespace: MassTransit.Configuration

```csharp
public class ConsumerEndpointDefinition<TConsumer> : SettingsEndpointDefinition<TConsumer>, IEndpointDefinition<TConsumer>, IEndpointDefinition
```

#### Type Parameters

`TConsumer`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SettingsEndpointDefinition\<TConsumer\>](../masstransit-configuration/settingsendpointdefinition-1) → [ConsumerEndpointDefinition\<TConsumer\>](../masstransit-configuration/consumerendpointdefinition-1)<br/>
Implements [IEndpointDefinition\<TConsumer\>](../masstransit/iendpointdefinition-1), [IEndpointDefinition](../masstransit/iendpointdefinition)

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

### **ConsumerEndpointDefinition(IEndpointSettings\<IEndpointDefinition\<TConsumer\>\>)**

```csharp
public ConsumerEndpointDefinition(IEndpointSettings<IEndpointDefinition<TConsumer>> settings)
```

#### Parameters

`settings` [IEndpointSettings\<IEndpointDefinition\<TConsumer\>\>](../masstransit/iendpointsettings-1)<br/>

## Methods

### **FormatEndpointName(IEndpointNameFormatter)**

```csharp
protected string FormatEndpointName(IEndpointNameFormatter formatter)
```

#### Parameters

`formatter` [IEndpointNameFormatter](../masstransit/iendpointnameformatter)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
