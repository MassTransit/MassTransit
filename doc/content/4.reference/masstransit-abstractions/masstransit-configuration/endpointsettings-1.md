---

title: EndpointSettings<TConsumer>

---

# EndpointSettings\<TConsumer\>

Namespace: MassTransit.Configuration

```csharp
public class EndpointSettings<TConsumer> : IEndpointSettings<TConsumer>
```

#### Type Parameters

`TConsumer`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EndpointSettings\<TConsumer\>](../masstransit-configuration/endpointsettings-1)<br/>
Implements [IEndpointSettings\<TConsumer\>](../masstransit/iendpointsettings-1)

## Properties

### **Name**

```csharp
public string Name { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **IsTemporary**

```csharp
public bool IsTemporary { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **PrefetchCount**

```csharp
public Nullable<int> PrefetchCount { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConfigureConsumeTopology**

```csharp
public bool ConfigureConsumeTopology { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **InstanceId**

```csharp
public string InstanceId { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **EndpointSettings()**

```csharp
public EndpointSettings()
```

## Methods

### **ConfigureEndpoint\<T\>(T, IRegistrationContext)**

```csharp
public void ConfigureEndpoint<T>(T configurator, IRegistrationContext context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` T<br/>

`context` [IRegistrationContext](../masstransit/iregistrationcontext)<br/>

### **AddConfigureEndpointCallback(Action\<IReceiveEndpointConfigurator\>)**

```csharp
public void AddConfigureEndpointCallback(Action<IReceiveEndpointConfigurator> callback)
```

#### Parameters

`callback` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **AddConfigureEndpointCallback(Action\<IRegistrationContext, IReceiveEndpointConfigurator\>)**

```csharp
public void AddConfigureEndpointCallback(Action<IRegistrationContext, IReceiveEndpointConfigurator> callback)
```

#### Parameters

`callback` [Action\<IRegistrationContext, IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>
