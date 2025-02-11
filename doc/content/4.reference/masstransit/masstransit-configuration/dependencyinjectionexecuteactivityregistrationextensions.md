---

title: DependencyInjectionExecuteActivityRegistrationExtensions

---

# DependencyInjectionExecuteActivityRegistrationExtensions

Namespace: MassTransit.Configuration

```csharp
public static class DependencyInjectionExecuteActivityRegistrationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DependencyInjectionExecuteActivityRegistrationExtensions](../masstransit-configuration/dependencyinjectionexecuteactivityregistrationextensions)

## Methods

### **RegisterExecuteActivity\<TActivity, TArguments\>(IServiceCollection)**

```csharp
public static IExecuteActivityRegistration RegisterExecuteActivity<TActivity, TArguments>(IServiceCollection collection)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`collection` IServiceCollection<br/>

#### Returns

[IExecuteActivityRegistration](../masstransit-configuration/iexecuteactivityregistration)<br/>

### **RegisterExecuteActivity\<TActivity, TArguments\>(IServiceCollection, IContainerRegistrar)**

```csharp
public static IExecuteActivityRegistration RegisterExecuteActivity<TActivity, TArguments>(IServiceCollection collection, IContainerRegistrar registrar)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

#### Returns

[IExecuteActivityRegistration](../masstransit-configuration/iexecuteactivityregistration)<br/>

### **RegisterExecuteActivity\<TActivity, TArguments, TDefinition\>(IServiceCollection)**

```csharp
public static IExecuteActivityRegistration RegisterExecuteActivity<TActivity, TArguments, TDefinition>(IServiceCollection collection)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

`TDefinition`<br/>

#### Parameters

`collection` IServiceCollection<br/>

#### Returns

[IExecuteActivityRegistration](../masstransit-configuration/iexecuteactivityregistration)<br/>

### **RegisterExecuteActivity\<TActivity, TArguments, TDefinition\>(IServiceCollection, IContainerRegistrar)**

```csharp
public static IExecuteActivityRegistration RegisterExecuteActivity<TActivity, TArguments, TDefinition>(IServiceCollection collection, IContainerRegistrar registrar)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

`TDefinition`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

#### Returns

[IExecuteActivityRegistration](../masstransit-configuration/iexecuteactivityregistration)<br/>

### **RegisterExecuteActivity\<TActivity, TArguments\>(IServiceCollection, Type)**

```csharp
public static IExecuteActivityRegistration RegisterExecuteActivity<TActivity, TArguments>(IServiceCollection collection, Type activityDefinitionType)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`activityDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IExecuteActivityRegistration](../masstransit-configuration/iexecuteactivityregistration)<br/>

### **RegisterExecuteActivity\<TActivity, TArguments\>(IServiceCollection, IContainerRegistrar, Type)**

```csharp
public static IExecuteActivityRegistration RegisterExecuteActivity<TActivity, TArguments>(IServiceCollection collection, IContainerRegistrar registrar, Type activityDefinitionType)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`activityDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IExecuteActivityRegistration](../masstransit-configuration/iexecuteactivityregistration)<br/>

### **RegisterExecuteActivity(IServiceCollection, IContainerRegistrar, Type, Type)**

```csharp
public static IExecuteActivityRegistration RegisterExecuteActivity(IServiceCollection collection, IContainerRegistrar registrar, Type activityType, Type activityDefinitionType)
```

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`activityType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`activityDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IExecuteActivityRegistration](../masstransit-configuration/iexecuteactivityregistration)<br/>
