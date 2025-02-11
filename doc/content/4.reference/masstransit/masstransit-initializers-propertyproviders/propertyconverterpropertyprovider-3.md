---

title: PropertyConverterPropertyProvider<TInput, TProperty, TInputProperty>

---

# PropertyConverterPropertyProvider\<TInput, TProperty, TInputProperty\>

Namespace: MassTransit.Initializers.PropertyProviders

```csharp
public class PropertyConverterPropertyProvider<TInput, TProperty, TInputProperty> : IPropertyProvider<TInput, TProperty>
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

`TInputProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PropertyConverterPropertyProvider\<TInput, TProperty, TInputProperty\>](../masstransit-initializers-propertyproviders/propertyconverterpropertyprovider-3)<br/>
Implements [IPropertyProvider\<TInput, TProperty\>](../masstransit-initializers/ipropertyprovider-2)

## Constructors

### **PropertyConverterPropertyProvider(IPropertyConverter\<TProperty, TInputProperty\>, IPropertyProvider\<TInput, TInputProperty\>)**

```csharp
public PropertyConverterPropertyProvider(IPropertyConverter<TProperty, TInputProperty> converter, IPropertyProvider<TInput, TInputProperty> inputProvider)
```

#### Parameters

`converter` [IPropertyConverter\<TProperty, TInputProperty\>](../masstransit-initializers/ipropertyconverter-2)<br/>

`inputProvider` [IPropertyProvider\<TInput, TInputProperty\>](../masstransit-initializers/ipropertyprovider-2)<br/>

## Methods

### **GetProperty\<T\>(InitializeContext\<T, TInput\>)**

```csharp
public Task<TProperty> GetProperty<T>(InitializeContext<T, TInput> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [InitializeContext\<T, TInput\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-2)<br/>

#### Returns

[Task\<TProperty\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
