---

title: InitializerConvention

---

# InitializerConvention

Namespace: MassTransit.Initializers.Conventions

```csharp
public abstract class InitializerConvention : IInitializerConvention
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InitializerConvention](../masstransit-initializers-conventions/initializerconvention)<br/>
Implements [IInitializerConvention](../masstransit-initializers-conventions/iinitializerconvention)

## Methods

### **TryGetPropertyInitializer\<TMessage, TInput, TProperty\>(PropertyInfo, IPropertyInitializer\<TMessage, TInput\>)**

```csharp
public bool TryGetPropertyInitializer<TMessage, TInput, TProperty>(PropertyInfo propertyInfo, out IPropertyInitializer<TMessage, TInput> initializer)
```

#### Type Parameters

`TMessage`<br/>

`TInput`<br/>

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`initializer` [IPropertyInitializer\<TMessage, TInput\>](../masstransit-initializers/ipropertyinitializer-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetHeaderInitializer\<TMessage, TInput, TProperty\>(PropertyInfo, IHeaderInitializer\<TMessage, TInput\>)**

```csharp
public bool TryGetHeaderInitializer<TMessage, TInput, TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
```

#### Type Parameters

`TMessage`<br/>

`TInput`<br/>

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`initializer` [IHeaderInitializer\<TMessage, TInput\>](../masstransit-initializers/iheaderinitializer-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetHeadersInitializer\<TMessage, TInput, TProperty\>(PropertyInfo, IHeaderInitializer\<TMessage, TInput\>)**

```csharp
public bool TryGetHeadersInitializer<TMessage, TInput, TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
```

#### Type Parameters

`TMessage`<br/>

`TInput`<br/>

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`initializer` [IHeaderInitializer\<TMessage, TInput\>](../masstransit-initializers/iheaderinitializer-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
