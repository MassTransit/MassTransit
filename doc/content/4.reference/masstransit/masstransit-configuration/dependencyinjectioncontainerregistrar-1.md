---

title: DependencyInjectionContainerRegistrar<TBus>

---

# DependencyInjectionContainerRegistrar\<TBus\>

Namespace: MassTransit.Configuration

```csharp
public class DependencyInjectionContainerRegistrar<TBus> : DependencyInjectionContainerRegistrar, IContainerRegistrar, IContainerSelector
```

#### Type Parameters

`TBus`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DependencyInjectionContainerRegistrar](../masstransit-configuration/dependencyinjectioncontainerregistrar) → [DependencyInjectionContainerRegistrar\<TBus\>](../masstransit-configuration/dependencyinjectioncontainerregistrar-1)<br/>
Implements [IContainerRegistrar](../masstransit-configuration/icontainerregistrar), [IContainerSelector](../masstransit-configuration/icontainerselector)

## Constructors

### **DependencyInjectionContainerRegistrar(IServiceCollection)**

```csharp
public DependencyInjectionContainerRegistrar(IServiceCollection collection)
```

#### Parameters

`collection` IServiceCollection<br/>

## Methods

### **GetRegistrations\<T\>()**

```csharp
public IEnumerable<T> GetRegistrations<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

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

### **AddRegistration\<T\>(T)**

```csharp
protected void AddRegistration<T>(T value)
```

#### Type Parameters

`T`<br/>

#### Parameters

`value` T<br/>

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

### **RegisterEndpointNameFormatter(IEndpointNameFormatter)**

```csharp
public void RegisterEndpointNameFormatter(IEndpointNameFormatter endpointNameFormatter)
```

#### Parameters

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

### **RegisterScopedClientFactory()**

```csharp
public void RegisterScopedClientFactory()
```

### **GetScopedBusContext(IServiceProvider)**

```csharp
protected IScopedClientFactory GetScopedBusContext(IServiceProvider provider)
```

#### Parameters

`provider` IServiceProvider<br/>

#### Returns

[IScopedClientFactory](../masstransit/iscopedclientfactory)<br/>

### **GetBusConfigureReceiveEndpoints(IServiceProvider)**

```csharp
protected IConfigureReceiveEndpoint[] GetBusConfigureReceiveEndpoints(IServiceProvider provider)
```

#### Parameters

`provider` IServiceProvider<br/>

#### Returns

[IConfigureReceiveEndpoint[]](../../masstransit-abstractions/masstransit/iconfigurereceiveendpoint)<br/>

### **GetEndpointNameFormatter(IServiceProvider)**

```csharp
public IEndpointNameFormatter GetEndpointNameFormatter(IServiceProvider provider)
```

#### Parameters

`provider` IServiceProvider<br/>

#### Returns

[IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>
