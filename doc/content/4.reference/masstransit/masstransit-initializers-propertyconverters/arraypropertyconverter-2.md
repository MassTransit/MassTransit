---

title: ArrayPropertyConverter<TElement, TInputElement>

---

# ArrayPropertyConverter\<TElement, TInputElement\>

Namespace: MassTransit.Initializers.PropertyConverters

```csharp
public class ArrayPropertyConverter<TElement, TInputElement> : IPropertyConverter<TElement[], IEnumerable<TInputElement>>
```

#### Type Parameters

`TElement`<br/>

`TInputElement`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ArrayPropertyConverter\<TElement, TInputElement\>](../masstransit-initializers-propertyconverters/arraypropertyconverter-2)<br/>
Implements [IPropertyConverter\<TElement[], IEnumerable\<TInputElement\>\>](../masstransit-initializers/ipropertyconverter-2)

## Constructors

### **ArrayPropertyConverter(IPropertyConverter\<TElement, TInputElement\>)**

```csharp
public ArrayPropertyConverter(IPropertyConverter<TElement, TInputElement> converter)
```

#### Parameters

`converter` [IPropertyConverter\<TElement, TInputElement\>](../masstransit-initializers/ipropertyconverter-2)<br/>

## Methods

### **Convert\<TMessage\>(InitializeContext\<TMessage\>, IEnumerable\<TInputElement\>)**

```csharp
public Task<TElement[]> Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<TInputElement> input)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`context` [InitializeContext\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` [IEnumerable\<TInputElement\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

#### Returns

[Task\<TElement[]\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
