---

title: DependencyInjectionFutureRegistrationExtensions

---

# DependencyInjectionFutureRegistrationExtensions

Namespace: MassTransit.Configuration

```csharp
public static class DependencyInjectionFutureRegistrationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DependencyInjectionFutureRegistrationExtensions](../masstransit-configuration/dependencyinjectionfutureregistrationextensions)

## Methods

### **RegisterFuture\<T\>(IServiceCollection)**

```csharp
public static IFutureRegistration RegisterFuture<T>(IServiceCollection collection)
```

#### Type Parameters

`T`<br/>

#### Parameters

`collection` IServiceCollection<br/>

#### Returns

[IFutureRegistration](../masstransit-configuration/ifutureregistration)<br/>

### **RegisterFuture\<T\>(IServiceCollection, IContainerRegistrar)**

```csharp
public static IFutureRegistration RegisterFuture<T>(IServiceCollection collection, IContainerRegistrar registrar)
```

#### Type Parameters

`T`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

#### Returns

[IFutureRegistration](../masstransit-configuration/ifutureregistration)<br/>

### **RegisterFuture\<T, TDefinition\>(IServiceCollection)**

```csharp
public static IFutureRegistration RegisterFuture<T, TDefinition>(IServiceCollection collection)
```

#### Type Parameters

`T`<br/>

`TDefinition`<br/>

#### Parameters

`collection` IServiceCollection<br/>

#### Returns

[IFutureRegistration](../masstransit-configuration/ifutureregistration)<br/>

### **RegisterFuture\<T, TDefinition\>(IServiceCollection, IContainerRegistrar)**

```csharp
public static IFutureRegistration RegisterFuture<T, TDefinition>(IServiceCollection collection, IContainerRegistrar registrar)
```

#### Type Parameters

`T`<br/>

`TDefinition`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

#### Returns

[IFutureRegistration](../masstransit-configuration/ifutureregistration)<br/>

### **RegisterFuture\<T\>(IServiceCollection, Type)**

```csharp
public static IFutureRegistration RegisterFuture<T>(IServiceCollection collection, Type futureDefinitionType)
```

#### Type Parameters

`T`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`futureDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IFutureRegistration](../masstransit-configuration/ifutureregistration)<br/>

### **RegisterFuture\<T\>(IServiceCollection, IContainerRegistrar, Type)**

```csharp
public static IFutureRegistration RegisterFuture<T>(IServiceCollection collection, IContainerRegistrar registrar, Type futureDefinitionType)
```

#### Type Parameters

`T`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`futureDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IFutureRegistration](../masstransit-configuration/ifutureregistration)<br/>

### **RegisterFuture(IServiceCollection, IContainerRegistrar, Type, Type)**

```csharp
public static IFutureRegistration RegisterFuture(IServiceCollection collection, IContainerRegistrar registrar, Type futureType, Type futureDefinitionType)
```

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`futureType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`futureDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IFutureRegistration](../masstransit-configuration/ifutureregistration)<br/>
