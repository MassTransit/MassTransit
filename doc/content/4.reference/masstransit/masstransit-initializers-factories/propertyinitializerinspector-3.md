---

title: PropertyInitializerInspector<TMessage, TInput, TProperty>

---

# PropertyInitializerInspector\<TMessage, TInput, TProperty\>

Namespace: MassTransit.Initializers.Factories

```csharp
public class PropertyInitializerInspector<TMessage, TInput, TProperty> : IPropertyInitializerInspector<TMessage, TInput>
```

#### Type Parameters

`TMessage`<br/>

`TInput`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PropertyInitializerInspector\<TMessage, TInput, TProperty\>](../masstransit-initializers-factories/propertyinitializerinspector-3)<br/>
Implements [IPropertyInitializerInspector\<TMessage, TInput\>](../masstransit-initializers-factories/ipropertyinitializerinspector-2)

## Constructors

### **PropertyInitializerInspector(PropertyInfo)**

```csharp
public PropertyInitializerInspector(PropertyInfo propertyInfo)
```

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

## Methods

### **Apply(IMessageInitializerBuilder\<TMessage, TInput\>, IInitializerConvention)**

```csharp
public bool Apply(IMessageInitializerBuilder<TMessage, TInput> builder, IInitializerConvention convention)
```

#### Parameters

`builder` [IMessageInitializerBuilder\<TMessage, TInput\>](../masstransit-initializers-factories/imessageinitializerbuilder-2)<br/>

`convention` [IInitializerConvention](../masstransit-initializers-conventions/iinitializerconvention)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
