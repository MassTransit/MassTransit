---

title: InputHeaderInitializerInspector<TMessage, TInput, TProperty>

---

# InputHeaderInitializerInspector\<TMessage, TInput, TProperty\>

Namespace: MassTransit.Initializers.Factories

```csharp
public class InputHeaderInitializerInspector<TMessage, TInput, TProperty> : IHeaderInitializerInspector<TMessage, TInput>
```

#### Type Parameters

`TMessage`<br/>

`TInput`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InputHeaderInitializerInspector\<TMessage, TInput, TProperty\>](../masstransit-initializers-factories/inputheaderinitializerinspector-3)<br/>
Implements [IHeaderInitializerInspector\<TMessage, TInput\>](../masstransit-initializers-factories/iheaderinitializerinspector-2)

## Constructors

### **InputHeaderInitializerInspector(PropertyInfo)**

```csharp
public InputHeaderInitializerInspector(PropertyInfo propertyInfo)
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
