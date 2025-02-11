---

title: IContainerRegistrar

---

# IContainerRegistrar

Namespace: MassTransit.Configuration

```csharp
public interface IContainerRegistrar : IContainerSelector
```

Implements [IContainerSelector](../masstransit-configuration/icontainerselector)

## Methods

### **RegisterRequestClient\<T\>(RequestTimeout)**

```csharp
void RegisterRequestClient<T>(RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

### **RegisterRequestClient\<T\>(Uri, RequestTimeout)**

```csharp
void RegisterRequestClient<T>(Uri destinationAddress, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

### **RegisterScopedClientFactory()**

```csharp
void RegisterScopedClientFactory()
```

### **RegisterEndpointNameFormatter(IEndpointNameFormatter)**

```csharp
void RegisterEndpointNameFormatter(IEndpointNameFormatter endpointNameFormatter)
```

#### Parameters

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

### **GetOrAddRegistration\<T\>(Type, Func\<Type, T\>)**

Gets or adds a registration from the service collection

```csharp
T GetOrAddRegistration<T>(Type type, Func<Type, T> missingRegistrationFactory)
```

#### Type Parameters

`T`<br/>
The registration type

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`missingRegistrationFactory` [Func\<Type, T\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

T<br/>

### **GetRegistrations\<T\>()**

Returns registrations from the service collection, prior to container construction

```csharp
IEnumerable<T> GetRegistrations<T>()
```

#### Type Parameters

`T`<br/>
The registration type

#### Returns

[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **AddDefinition\<T, TDefinition\>()**

Gets or adds a definition from the service collection

```csharp
void AddDefinition<T, TDefinition>()
```

#### Type Parameters

`T`<br/>
The definition type

`TDefinition`<br/>
The definition implementation

### **AddEndpointDefinition\<T, TDefinition\>(IEndpointSettings\<IEndpointDefinition\<T\>\>)**

```csharp
void AddEndpointDefinition<T, TDefinition>(IEndpointSettings<IEndpointDefinition<T>> settings)
```

#### Type Parameters

`T`<br/>

`TDefinition`<br/>

#### Parameters

`settings` [IEndpointSettings\<IEndpointDefinition\<T\>\>](../../masstransit-abstractions/masstransit/iendpointsettings-1)<br/>
