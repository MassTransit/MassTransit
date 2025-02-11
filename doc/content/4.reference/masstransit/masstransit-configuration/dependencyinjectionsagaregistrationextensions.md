---

title: DependencyInjectionSagaRegistrationExtensions

---

# DependencyInjectionSagaRegistrationExtensions

Namespace: MassTransit.Configuration

```csharp
public static class DependencyInjectionSagaRegistrationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DependencyInjectionSagaRegistrationExtensions](../masstransit-configuration/dependencyinjectionsagaregistrationextensions)

## Methods

### **RegisterSaga\<T\>(IServiceCollection)**

```csharp
public static ISagaRegistration RegisterSaga<T>(IServiceCollection collection)
```

#### Type Parameters

`T`<br/>

#### Parameters

`collection` IServiceCollection<br/>

#### Returns

[ISagaRegistration](../masstransit-configuration/isagaregistration)<br/>

### **RegisterSaga\<T\>(IServiceCollection, IContainerRegistrar)**

```csharp
public static ISagaRegistration RegisterSaga<T>(IServiceCollection collection, IContainerRegistrar registrar)
```

#### Type Parameters

`T`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

#### Returns

[ISagaRegistration](../masstransit-configuration/isagaregistration)<br/>

### **RegisterSaga\<T, TDefinition\>(IServiceCollection)**

```csharp
public static ISagaRegistration RegisterSaga<T, TDefinition>(IServiceCollection collection)
```

#### Type Parameters

`T`<br/>

`TDefinition`<br/>

#### Parameters

`collection` IServiceCollection<br/>

#### Returns

[ISagaRegistration](../masstransit-configuration/isagaregistration)<br/>

### **RegisterSaga\<T, TDefinition\>(IServiceCollection, IContainerRegistrar)**

```csharp
public static ISagaRegistration RegisterSaga<T, TDefinition>(IServiceCollection collection, IContainerRegistrar registrar)
```

#### Type Parameters

`T`<br/>

`TDefinition`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

#### Returns

[ISagaRegistration](../masstransit-configuration/isagaregistration)<br/>

### **RegisterSaga\<T\>(IServiceCollection, Type)**

```csharp
public static ISagaRegistration RegisterSaga<T>(IServiceCollection collection, Type sagaDefinitionType)
```

#### Type Parameters

`T`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`sagaDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[ISagaRegistration](../masstransit-configuration/isagaregistration)<br/>

### **RegisterSaga\<T\>(IServiceCollection, IContainerRegistrar, Type)**

```csharp
public static ISagaRegistration RegisterSaga<T>(IServiceCollection collection, IContainerRegistrar registrar, Type sagaDefinitionType)
```

#### Type Parameters

`T`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`sagaDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[ISagaRegistration](../masstransit-configuration/isagaregistration)<br/>

### **RegisterSaga(IServiceCollection, IContainerRegistrar, Type, Type)**

```csharp
public static ISagaRegistration RegisterSaga(IServiceCollection collection, IContainerRegistrar registrar, Type sagaType, Type sagaDefinitionType)
```

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`sagaType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`sagaDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[ISagaRegistration](../masstransit-configuration/isagaregistration)<br/>
