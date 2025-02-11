---

title: ProviderPropertyInitializer<TMessage, TInput, TProperty>

---

# ProviderPropertyInitializer\<TMessage, TInput, TProperty\>

Namespace: MassTransit.Initializers.PropertyInitializers

Set a message property using the property provider for the property value

```csharp
public class ProviderPropertyInitializer<TMessage, TInput, TProperty> : IPropertyInitializer<TMessage, TInput>
```

#### Type Parameters

`TMessage`<br/>

`TInput`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ProviderPropertyInitializer\<TMessage, TInput, TProperty\>](../masstransit-initializers-propertyinitializers/providerpropertyinitializer-3)<br/>
Implements [IPropertyInitializer\<TMessage, TInput\>](../masstransit-initializers/ipropertyinitializer-2)

## Constructors

### **ProviderPropertyInitializer(IPropertyProvider\<TInput, TProperty\>, PropertyInfo)**

```csharp
public ProviderPropertyInitializer(IPropertyProvider<TInput, TProperty> propertyProvider, PropertyInfo propertyInfo)
```

#### Parameters

`propertyProvider` [IPropertyProvider\<TInput, TProperty\>](../masstransit-initializers/ipropertyprovider-2)<br/>

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

## Methods

### **Apply(InitializeContext\<TMessage, TInput\>)**

```csharp
public Task Apply(InitializeContext<TMessage, TInput> context)
```

#### Parameters

`context` [InitializeContext\<TMessage, TInput\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
