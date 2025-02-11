---

title: IContainerSelector

---

# IContainerSelector

Namespace: MassTransit.Configuration

Used to pull configuration from the container, scoped to the bus, multi-bus, or mediator

```csharp
public interface IContainerSelector
```

## Methods

### **TryGetRegistration\<T\>(IServiceProvider, Type, T)**

Returns the registration from the service provider, if it exists

```csharp
bool TryGetRegistration<T>(IServiceProvider provider, Type type, out T value)
```

#### Type Parameters

`T`<br/>
The registration type

#### Parameters

`provider` IServiceProvider<br/>
The service provider

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The registration target type (Consumer, Saga, Activity, etc.)

`value` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetRegistrations\<T\>(IServiceProvider)**

```csharp
IEnumerable<T> GetRegistrations<T>(IServiceProvider provider)
```

#### Type Parameters

`T`<br/>

#### Parameters

`provider` IServiceProvider<br/>

#### Returns

[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **GetDefinition\<T\>(IServiceProvider)**

Returns the definition from the service provider, if it exists

```csharp
T GetDefinition<T>(IServiceProvider provider)
```

#### Type Parameters

`T`<br/>
The definition type

#### Parameters

`provider` IServiceProvider<br/>
The service provider

#### Returns

T<br/>
The definition, if found, otherwise null

### **GetEndpointDefinition\<T\>(IServiceProvider)**

Returns the endpoint definition from the service provider, if it exists

```csharp
IEndpointDefinition<T> GetEndpointDefinition<T>(IServiceProvider provider)
```

#### Type Parameters

`T`<br/>
The definition target type

#### Parameters

`provider` IServiceProvider<br/>
The service provider

#### Returns

[IEndpointDefinition\<T\>](../../masstransit-abstractions/masstransit/iendpointdefinition-1)<br/>

### **GetConfigureReceiveEndpoints(IServiceProvider)**

```csharp
IConfigureReceiveEndpoint GetConfigureReceiveEndpoints(IServiceProvider provider)
```

#### Parameters

`provider` IServiceProvider<br/>

#### Returns

[IConfigureReceiveEndpoint](../../masstransit-abstractions/masstransit/iconfigurereceiveendpoint)<br/>

### **GetEndpointNameFormatter(IServiceProvider)**

Returns the endpoint name formatter registered for the bus instance

```csharp
IEndpointNameFormatter GetEndpointNameFormatter(IServiceProvider provider)
```

#### Parameters

`provider` IServiceProvider<br/>

#### Returns

[IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>
