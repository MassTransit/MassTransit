---

title: ProviderHeaderInitializer<TMessage, TInput, TProperty>

---

# ProviderHeaderInitializer\<TMessage, TInput, TProperty\>

Namespace: MassTransit.Initializers.HeaderInitializers

Set a message property using the property provider for the property value

```csharp
public class ProviderHeaderInitializer<TMessage, TInput, TProperty> : IHeaderInitializer<TMessage, TInput>
```

#### Type Parameters

`TMessage`<br/>

`TInput`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ProviderHeaderInitializer\<TMessage, TInput, TProperty\>](../masstransit-initializers-headerinitializers/providerheaderinitializer-3)<br/>
Implements [IHeaderInitializer\<TMessage, TInput\>](../masstransit-initializers/iheaderinitializer-2)

## Constructors

### **ProviderHeaderInitializer(IPropertyProvider\<TInput, TProperty\>, PropertyInfo)**

```csharp
public ProviderHeaderInitializer(IPropertyProvider<TInput, TProperty> propertyProvider, PropertyInfo propertyInfo)
```

#### Parameters

`propertyProvider` [IPropertyProvider\<TInput, TProperty\>](../masstransit-initializers/ipropertyprovider-2)<br/>

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

## Methods

### **Apply(InitializeContext\<TMessage, TInput\>, SendContext)**

```csharp
public Task Apply(InitializeContext<TMessage, TInput> context, SendContext sendContext)
```

#### Parameters

`context` [InitializeContext\<TMessage, TInput\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-2)<br/>

`sendContext` [SendContext](../../masstransit-abstractions/masstransit/sendcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
