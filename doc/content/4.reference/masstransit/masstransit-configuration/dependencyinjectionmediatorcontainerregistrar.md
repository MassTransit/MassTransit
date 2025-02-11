---

title: DependencyInjectionMediatorContainerRegistrar

---

# DependencyInjectionMediatorContainerRegistrar

Namespace: MassTransit.Configuration

```csharp
public class DependencyInjectionMediatorContainerRegistrar : DependencyInjectionContainerRegistrar, IContainerRegistrar, IContainerSelector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DependencyInjectionContainerRegistrar](../masstransit-configuration/dependencyinjectioncontainerregistrar) → [DependencyInjectionMediatorContainerRegistrar](../masstransit-configuration/dependencyinjectionmediatorcontainerregistrar)<br/>
Implements [IContainerRegistrar](../masstransit-configuration/icontainerregistrar), [IContainerSelector](../masstransit-configuration/icontainerselector)

## Constructors

### **DependencyInjectionMediatorContainerRegistrar(IServiceCollection)**

```csharp
public DependencyInjectionMediatorContainerRegistrar(IServiceCollection collection)
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

### **GetScopedBusContext(IServiceProvider)**

```csharp
protected IScopedClientFactory GetScopedBusContext(IServiceProvider provider)
```

#### Parameters

`provider` IServiceProvider<br/>

#### Returns

[IScopedClientFactory](../masstransit/iscopedclientfactory)<br/>
