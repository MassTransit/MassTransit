---

title: DependencyInjectionActivityRegistrationExtensions

---

# DependencyInjectionActivityRegistrationExtensions

Namespace: MassTransit.Configuration

```csharp
public static class DependencyInjectionActivityRegistrationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DependencyInjectionActivityRegistrationExtensions](../masstransit-configuration/dependencyinjectionactivityregistrationextensions)

## Methods

### **RegisterActivity\<TActivity, TArguments, TLog\>(IServiceCollection)**

```csharp
public static IActivityRegistration RegisterActivity<TActivity, TArguments, TLog>(IServiceCollection collection)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

`TLog`<br/>

#### Parameters

`collection` IServiceCollection<br/>

#### Returns

[IActivityRegistration](../masstransit-configuration/iactivityregistration)<br/>

### **RegisterActivity\<TActivity, TArguments, TLog\>(IServiceCollection, IContainerRegistrar)**

```csharp
public static IActivityRegistration RegisterActivity<TActivity, TArguments, TLog>(IServiceCollection collection, IContainerRegistrar registrar)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

`TLog`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

#### Returns

[IActivityRegistration](../masstransit-configuration/iactivityregistration)<br/>

### **RegisterActivity\<TActivity, TArguments, TLog, TDefinition\>(IServiceCollection)**

```csharp
public static IActivityRegistration RegisterActivity<TActivity, TArguments, TLog, TDefinition>(IServiceCollection collection)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

`TLog`<br/>

`TDefinition`<br/>

#### Parameters

`collection` IServiceCollection<br/>

#### Returns

[IActivityRegistration](../masstransit-configuration/iactivityregistration)<br/>

### **RegisterActivity\<TActivity, TArguments, TLog, TDefinition\>(IServiceCollection, IContainerRegistrar)**

```csharp
public static IActivityRegistration RegisterActivity<TActivity, TArguments, TLog, TDefinition>(IServiceCollection collection, IContainerRegistrar registrar)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

`TLog`<br/>

`TDefinition`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

#### Returns

[IActivityRegistration](../masstransit-configuration/iactivityregistration)<br/>

### **RegisterActivity\<TActivity, TArguments, TLog\>(IServiceCollection, Type)**

```csharp
public static IActivityRegistration RegisterActivity<TActivity, TArguments, TLog>(IServiceCollection collection, Type activityDefinitionType)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

`TLog`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`activityDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IActivityRegistration](../masstransit-configuration/iactivityregistration)<br/>

### **RegisterActivity\<TActivity, TArguments, TLog\>(IServiceCollection, IContainerRegistrar, Type)**

```csharp
public static IActivityRegistration RegisterActivity<TActivity, TArguments, TLog>(IServiceCollection collection, IContainerRegistrar registrar, Type activityDefinitionType)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

`TLog`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`activityDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IActivityRegistration](../masstransit-configuration/iactivityregistration)<br/>

### **RegisterActivity(IServiceCollection, IContainerRegistrar, Type, Type)**

```csharp
public static IActivityRegistration RegisterActivity(IServiceCollection collection, IContainerRegistrar registrar, Type activityType, Type activityDefinitionType)
```

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`activityType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`activityDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IActivityRegistration](../masstransit-configuration/iactivityregistration)<br/>
