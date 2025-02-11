---

title: EndpointRegistrationConfigurator<T>

---

# EndpointRegistrationConfigurator\<T\>

Namespace: MassTransit.Configuration

```csharp
public class EndpointRegistrationConfigurator<T> : IEndpointRegistrationConfigurator
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EndpointRegistrationConfigurator\<T\>](../masstransit-configuration/endpointregistrationconfigurator-1)<br/>
Implements [IEndpointRegistrationConfigurator](../masstransit/iendpointregistrationconfigurator)

## Properties

### **Settings**

```csharp
public IEndpointSettings<IEndpointDefinition<T>> Settings { get; }
```

#### Property Value

[IEndpointSettings\<IEndpointDefinition\<T\>\>](../masstransit/iendpointsettings-1)<br/>

### **Name**

```csharp
public string Name { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Temporary**

```csharp
public bool Temporary { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **PrefetchCount**

```csharp
public Nullable<int> PrefetchCount { set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConfigureConsumeTopology**

```csharp
public bool ConfigureConsumeTopology { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **InstanceId**

```csharp
public string InstanceId { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **EndpointRegistrationConfigurator()**

```csharp
public EndpointRegistrationConfigurator()
```

## Methods

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
