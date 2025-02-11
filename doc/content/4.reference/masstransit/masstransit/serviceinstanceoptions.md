---

title: ServiceInstanceOptions

---

# ServiceInstanceOptions

Namespace: MassTransit

```csharp
public class ServiceInstanceOptions : OptionsSet, IOptionsSet
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [OptionsSet](../../masstransit-abstractions/masstransit-configuration/optionsset) → [ServiceInstanceOptions](../masstransit/serviceinstanceoptions)<br/>
Implements [IOptionsSet](../../masstransit-abstractions/masstransit-configuration/ioptionsset)

## Properties

### **EndpointNameFormatter**

```csharp
public IEndpointNameFormatter EndpointNameFormatter { get; private set; }
```

#### Property Value

[IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

## Constructors

### **ServiceInstanceOptions()**

```csharp
public ServiceInstanceOptions()
```

## Methods

### **EnableJobServiceEndpoints()**

Enable the job service endpoints, so that  consumers
 can be configured.

```csharp
public ServiceInstanceOptions EnableJobServiceEndpoints()
```

#### Returns

[ServiceInstanceOptions](../masstransit/serviceinstanceoptions)<br/>

### **SetEndpointNameFormatter(IEndpointNameFormatter)**

```csharp
public ServiceInstanceOptions SetEndpointNameFormatter(IEndpointNameFormatter endpointNameFormatter)
```

#### Parameters

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

#### Returns

[ServiceInstanceOptions](../masstransit/serviceinstanceoptions)<br/>
