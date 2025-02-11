---

title: IInitializerConvention<TMessage>

---

# IInitializerConvention\<TMessage\>

Namespace: MassTransit.Initializers.Conventions

```csharp
public interface IInitializerConvention<TMessage> : IMessageInitializerConvention
```

#### Type Parameters

`TMessage`<br/>

Implements [IMessageInitializerConvention](../masstransit-initializers-conventions/imessageinitializerconvention)

## Methods

### **TryGetPropertyInitializer\<TInput, TProperty\>(PropertyInfo, IPropertyInitializer\<TMessage, TInput\>)**

```csharp
bool TryGetPropertyInitializer<TInput, TProperty>(PropertyInfo propertyInfo, out IPropertyInitializer<TMessage, TInput> initializer)
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
bool TryGetHeaderInitializer<TInput, TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
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
bool TryGetHeadersInitializer<TInput, TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`initializer` [IHeaderInitializer\<TMessage, TInput\>](../masstransit-initializers/iheaderinitializer-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
