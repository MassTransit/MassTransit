---

title: DependencyInjectionRiderContainerRegistrar<TBus>

---

# DependencyInjectionRiderContainerRegistrar\<TBus\>

Namespace: MassTransit.Configuration

```csharp
public class DependencyInjectionRiderContainerRegistrar<TBus> : DependencyInjectionContainerRegistrar, IContainerRegistrar, IContainerSelector
```

#### Type Parameters

`TBus`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DependencyInjectionContainerRegistrar](../masstransit-configuration/dependencyinjectioncontainerregistrar) → [DependencyInjectionRiderContainerRegistrar\<TBus\>](../masstransit-configuration/dependencyinjectionridercontainerregistrar-1)<br/>
Implements [IContainerRegistrar](../masstransit-configuration/icontainerregistrar), [IContainerSelector](../masstransit-configuration/icontainerselector)

## Constructors

### **DependencyInjectionRiderContainerRegistrar(IServiceCollection)**

```csharp
public DependencyInjectionRiderContainerRegistrar(IServiceCollection collection)
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
