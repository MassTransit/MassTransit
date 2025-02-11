---

title: RegistrationCache<T>

---

# RegistrationCache\<T\>

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public class RegistrationCache<T> : IRegistrationCache<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RegistrationCache\<T\>](../masstransit-dependencyinjection-registration/registrationcache-1)<br/>
Implements [IRegistrationCache\<T\>](../masstransit-configuration/iregistrationcache-1)

## Properties

### **Values**

```csharp
public IEnumerable<T> Values { get; }
```

#### Property Value

[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Constructors

### **RegistrationCache(Func\<Type, T\>)**

```csharp
public RegistrationCache(Func<Type, T> missingRegistrationFactory)
```

#### Parameters

`missingRegistrationFactory` [Func\<Type, T\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

## Methods

### **GetOrAdd(Type, Func\<Type, T\>)**

```csharp
public T GetOrAdd(Type type, Func<Type, T> missingRegistrationFactory)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`missingRegistrationFactory` [Func\<Type, T\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

T<br/>
