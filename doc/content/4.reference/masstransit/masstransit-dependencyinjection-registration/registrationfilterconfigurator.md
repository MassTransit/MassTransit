---

title: RegistrationFilterConfigurator

---

# RegistrationFilterConfigurator

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public class RegistrationFilterConfigurator : IRegistrationFilterConfigurator
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RegistrationFilterConfigurator](../masstransit-dependencyinjection-registration/registrationfilterconfigurator)<br/>
Implements [IRegistrationFilterConfigurator](../masstransit-dependencyinjection-registration/iregistrationfilterconfigurator)

## Properties

### **Filter**

```csharp
public IRegistrationFilter Filter { get; }
```

#### Property Value

[IRegistrationFilter](../masstransit-dependencyinjection-registration/iregistrationfilter)<br/>

## Constructors

### **RegistrationFilterConfigurator()**

```csharp
public RegistrationFilterConfigurator()
```

## Methods

### **Include(Type[])**

```csharp
public void Include(Type[] types)
```

#### Parameters

`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **Include\<T\>()**

```csharp
public void Include<T>()
```

#### Type Parameters

`T`<br/>

### **Exclude(Type[])**

```csharp
public void Exclude(Type[] types)
```

#### Parameters

`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **Exclude\<T\>()**

```csharp
public void Exclude<T>()
```

#### Type Parameters

`T`<br/>
