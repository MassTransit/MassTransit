---

title: DependencyInjectionConsumerRegistrationExtensions

---

# DependencyInjectionConsumerRegistrationExtensions

Namespace: MassTransit.Configuration

```csharp
public static class DependencyInjectionConsumerRegistrationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DependencyInjectionConsumerRegistrationExtensions](../masstransit-configuration/dependencyinjectionconsumerregistrationextensions)

## Methods

### **RegisterConsumer\<T\>(IServiceCollection)**

```csharp
public static IConsumerRegistration RegisterConsumer<T>(IServiceCollection collection)
```

#### Type Parameters

`T`<br/>

#### Parameters

`collection` IServiceCollection<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterConsumer\<T\>(IServiceCollection, IContainerRegistrar)**

```csharp
public static IConsumerRegistration RegisterConsumer<T>(IServiceCollection collection, IContainerRegistrar registrar)
```

#### Type Parameters

`T`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterConsumer\<T, TDefinition\>(IServiceCollection)**

```csharp
public static IConsumerRegistration RegisterConsumer<T, TDefinition>(IServiceCollection collection)
```

#### Type Parameters

`T`<br/>

`TDefinition`<br/>

#### Parameters

`collection` IServiceCollection<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterConsumer\<T, TDefinition\>(IServiceCollection, IContainerRegistrar)**

```csharp
public static IConsumerRegistration RegisterConsumer<T, TDefinition>(IServiceCollection collection, IContainerRegistrar registrar)
```

#### Type Parameters

`T`<br/>

`TDefinition`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterConsumer\<T\>(IServiceCollection, Type)**

```csharp
public static IConsumerRegistration RegisterConsumer<T>(IServiceCollection collection, Type consumerDefinitionType)
```

#### Type Parameters

`T`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`consumerDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterConsumer\<T\>(IServiceCollection, IContainerRegistrar, Type)**

```csharp
public static IConsumerRegistration RegisterConsumer<T>(IServiceCollection collection, IContainerRegistrar registrar, Type consumerDefinitionType)
```

#### Type Parameters

`T`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`consumerDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterConsumer(IServiceCollection, IContainerRegistrar, Type, Type)**

```csharp
public static IConsumerRegistration RegisterConsumer(IServiceCollection collection, IContainerRegistrar registrar, Type consumerType, Type consumerDefinitionType)
```

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`consumerType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`consumerDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>
