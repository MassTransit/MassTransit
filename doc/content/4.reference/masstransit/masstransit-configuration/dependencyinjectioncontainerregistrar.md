---

title: DependencyInjectionContainerRegistrar

---

# DependencyInjectionContainerRegistrar

Namespace: MassTransit.Configuration

```csharp
public class DependencyInjectionContainerRegistrar : IContainerRegistrar, IContainerSelector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DependencyInjectionContainerRegistrar](../masstransit-configuration/dependencyinjectioncontainerregistrar)<br/>
Implements [IContainerRegistrar](../masstransit-configuration/icontainerregistrar), [IContainerSelector](../masstransit-configuration/icontainerselector)

## Constructors

### **DependencyInjectionContainerRegistrar(IServiceCollection)**

```csharp
public DependencyInjectionContainerRegistrar(IServiceCollection collection)
```

#### Parameters

`collection` IServiceCollection<br/>

## Methods

### **RegisterRequestClient\<T\>(RequestTimeout)**

```csharp
public void RegisterRequestClient<T>(RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

### **RegisterRequestClient\<T\>(Uri, RequestTimeout)**

```csharp
public void RegisterRequestClient<T>(Uri destinationAddress, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

### **RegisterScopedClientFactory()**

```csharp
public void RegisterScopedClientFactory()
```

### **RegisterEndpointNameFormatter(IEndpointNameFormatter)**

```csharp
public void RegisterEndpointNameFormatter(IEndpointNameFormatter endpointNameFormatter)
```

#### Parameters

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

### **GetOrAddRegistration\<T\>(Type, Func\<Type, T\>)**

```csharp
public T GetOrAddRegistration<T>(Type type, Func<Type, T> missingRegistrationFactory)
```

#### Type Parameters

`T`<br/>

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`missingRegistrationFactory` [Func\<Type, T\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

T<br/>

### **AddDefinition\<T, TDefinition\>()**

```csharp
public void AddDefinition<T, TDefinition>()
```

#### Type Parameters

`T`<br/>

`TDefinition`<br/>

### **AddEndpointDefinition\<T, TDefinition\>(IEndpointSettings\<IEndpointDefinition\<T\>\>)**

```csharp
public void AddEndpointDefinition<T, TDefinition>(IEndpointSettings<IEndpointDefinition<T>> settings)
```

#### Type Parameters

`T`<br/>

`TDefinition`<br/>

#### Parameters

`settings` [IEndpointSettings\<IEndpointDefinition\<T\>\>](../../masstransit-abstractions/masstransit/iendpointsettings-1)<br/>

### **GetRegistrations\<T\>()**

```csharp
public IEnumerable<T> GetRegistrations<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **TryGetRegistration\<T\>(IServiceProvider, Type, T)**

```csharp
public bool TryGetRegistration<T>(IServiceProvider provider, Type type, out T value)
```

#### Type Parameters

`T`<br/>

#### Parameters

`provider` IServiceProvider<br/>

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`value` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetRegistrations\<T\>(IServiceProvider)**

```csharp
public IEnumerable<T> GetRegistrations<T>(IServiceProvider provider)
```

#### Type Parameters

`T`<br/>

#### Parameters

`provider` IServiceProvider<br/>

#### Returns

[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **GetDefinition\<T\>(IServiceProvider)**

```csharp
public T GetDefinition<T>(IServiceProvider provider)
```

#### Type Parameters

`T`<br/>

#### Parameters

`provider` IServiceProvider<br/>

#### Returns

T<br/>

### **GetEndpointDefinition\<T\>(IServiceProvider)**

```csharp
public IEndpointDefinition<T> GetEndpointDefinition<T>(IServiceProvider provider)
```

#### Type Parameters

`T`<br/>

#### Parameters

`provider` IServiceProvider<br/>

#### Returns

[IEndpointDefinition\<T\>](../../masstransit-abstractions/masstransit/iendpointdefinition-1)<br/>

### **GetConfigureReceiveEndpoints(IServiceProvider)**

```csharp
public IConfigureReceiveEndpoint GetConfigureReceiveEndpoints(IServiceProvider provider)
```

#### Parameters

`provider` IServiceProvider<br/>

#### Returns

[IConfigureReceiveEndpoint](../../masstransit-abstractions/masstransit/iconfigurereceiveendpoint)<br/>

### **GetEndpointNameFormatter(IServiceProvider)**

```csharp
public IEndpointNameFormatter GetEndpointNameFormatter(IServiceProvider provider)
```

#### Parameters

`provider` IServiceProvider<br/>

#### Returns

[IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

### **GetBusConfigureReceiveEndpoints(IServiceProvider)**

```csharp
protected IConfigureReceiveEndpoint[] GetBusConfigureReceiveEndpoints(IServiceProvider provider)
```

#### Parameters

`provider` IServiceProvider<br/>

#### Returns

[IConfigureReceiveEndpoint[]](../../masstransit-abstractions/masstransit/iconfigurereceiveendpoint)<br/>

### **AddRegistration\<T\>(T)**

```csharp
protected void AddRegistration<T>(T value)
```

#### Type Parameters

`T`<br/>

#### Parameters

`value` T<br/>

### **GetScopedBusContext(IServiceProvider)**

```csharp
protected IScopedClientFactory GetScopedBusContext(IServiceProvider provider)
```

#### Parameters

`provider` IServiceProvider<br/>

#### Returns

[IScopedClientFactory](../masstransit/iscopedclientfactory)<br/>
