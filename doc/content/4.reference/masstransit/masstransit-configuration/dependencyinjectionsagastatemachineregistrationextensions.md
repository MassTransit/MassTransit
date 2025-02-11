---

title: DependencyInjectionSagaStateMachineRegistrationExtensions

---

# DependencyInjectionSagaStateMachineRegistrationExtensions

Namespace: MassTransit.Configuration

```csharp
public static class DependencyInjectionSagaStateMachineRegistrationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DependencyInjectionSagaStateMachineRegistrationExtensions](../masstransit-configuration/dependencyinjectionsagastatemachineregistrationextensions)

## Methods

### **RegisterSagaStateMachine\<T, TSaga\>(IServiceCollection)**

```csharp
public static ISagaRegistration RegisterSagaStateMachine<T, TSaga>(IServiceCollection collection)
```

#### Type Parameters

`T`<br/>

`TSaga`<br/>

#### Parameters

`collection` IServiceCollection<br/>

#### Returns

[ISagaRegistration](../masstransit-configuration/isagaregistration)<br/>

### **RegisterSagaStateMachine\<T, TSaga\>(IServiceCollection, IContainerRegistrar)**

```csharp
public static ISagaRegistration RegisterSagaStateMachine<T, TSaga>(IServiceCollection collection, IContainerRegistrar registrar)
```

#### Type Parameters

`T`<br/>

`TSaga`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

#### Returns

[ISagaRegistration](../masstransit-configuration/isagaregistration)<br/>

### **RegisterSaga\<T, TSaga, TDefinition\>(IServiceCollection)**

```csharp
public static ISagaRegistration RegisterSaga<T, TSaga, TDefinition>(IServiceCollection collection)
```

#### Type Parameters

`T`<br/>

`TSaga`<br/>

`TDefinition`<br/>

#### Parameters

`collection` IServiceCollection<br/>

#### Returns

[ISagaRegistration](../masstransit-configuration/isagaregistration)<br/>

### **RegisterSaga\<T, TSaga, TDefinition\>(IServiceCollection, IContainerRegistrar)**

```csharp
public static ISagaRegistration RegisterSaga<T, TSaga, TDefinition>(IServiceCollection collection, IContainerRegistrar registrar)
```

#### Type Parameters

`T`<br/>

`TSaga`<br/>

`TDefinition`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

#### Returns

[ISagaRegistration](../masstransit-configuration/isagaregistration)<br/>

### **RegisterSagaStateMachine\<T, TSaga\>(IServiceCollection, Type)**

```csharp
public static ISagaRegistration RegisterSagaStateMachine<T, TSaga>(IServiceCollection collection, Type sagaDefinitionType)
```

#### Type Parameters

`T`<br/>

`TSaga`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`sagaDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[ISagaRegistration](../masstransit-configuration/isagaregistration)<br/>

### **RegisterSagaStateMachine\<T, TSaga\>(IServiceCollection, IContainerRegistrar, Type)**

```csharp
public static ISagaRegistration RegisterSagaStateMachine<T, TSaga>(IServiceCollection collection, IContainerRegistrar registrar, Type sagaDefinitionType)
```

#### Type Parameters

`T`<br/>

`TSaga`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`sagaDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[ISagaRegistration](../masstransit-configuration/isagaregistration)<br/>

### **RegisterSagaStateMachine(IServiceCollection, IContainerRegistrar, Type, Type)**

```csharp
public static ISagaRegistration RegisterSagaStateMachine(IServiceCollection collection, IContainerRegistrar registrar, Type sagaType, Type sagaDefinitionType)
```

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`sagaType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`sagaDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[ISagaRegistration](../masstransit-configuration/isagaregistration)<br/>
