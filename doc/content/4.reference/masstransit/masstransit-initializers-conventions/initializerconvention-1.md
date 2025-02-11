---

title: InitializerConvention<TMessage>

---

# InitializerConvention\<TMessage\>

Namespace: MassTransit.Initializers.Conventions

```csharp
public abstract class InitializerConvention<TMessage> : IInitializerConvention<TMessage>, IMessageInitializerConvention
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InitializerConvention\<TMessage\>](../masstransit-initializers-conventions/initializerconvention-1)<br/>
Implements [IInitializerConvention\<TMessage\>](../masstransit-initializers-conventions/iinitializerconvention-1), [IMessageInitializerConvention](../masstransit-initializers-conventions/imessageinitializerconvention)

## Methods

### **TryGetPropertyInitializer\<TInput, TProperty\>(PropertyInfo, IPropertyInitializer\<TMessage, TInput\>)**

```csharp
public bool TryGetPropertyInitializer<TInput, TProperty>(PropertyInfo propertyInfo, out IPropertyInitializer<TMessage, TInput> initializer)
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`initializer` [IPropertyInitializer\<TMessage, TInput\>](../masstransit-initializers/ipropertyinitializer-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetHeaderInitializer\<TInput, TProperty\>(PropertyInfo, IHeaderInitializer\<TMessage, TInput\>)**

```csharp
public bool TryGetHeaderInitializer<TInput, TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`initializer` [IHeaderInitializer\<TMessage, TInput\>](../masstransit-initializers/iheaderinitializer-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetHeadersInitializer\<TInput, TProperty\>(PropertyInfo, IHeaderInitializer\<TMessage, TInput\>)**

```csharp
public bool TryGetHeadersInitializer<TInput, TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`initializer` [IHeaderInitializer\<TMessage, TInput\>](../masstransit-initializers/iheaderinitializer-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
