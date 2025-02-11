---

title: VariablePropertyConverter<TResult, TVariable, TValue>

---

# VariablePropertyConverter\<TResult, TVariable, TValue\>

Namespace: MassTransit.Initializers.PropertyConverters

```csharp
public class VariablePropertyConverter<TResult, TVariable, TValue> : IPropertyConverter<TResult, TVariable>
```

#### Type Parameters

`TResult`<br/>

`TVariable`<br/>

`TValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [VariablePropertyConverter\<TResult, TVariable, TValue\>](../masstransit-initializers-propertyconverters/variablepropertyconverter-3)<br/>
Implements [IPropertyConverter\<TResult, TVariable\>](../masstransit-initializers/ipropertyconverter-2)

## Constructors

### **VariablePropertyConverter(IPropertyConverter\<TResult, TValue\>)**

```csharp
public VariablePropertyConverter(IPropertyConverter<TResult, TValue> propertyConverter)
```

#### Parameters

`propertyConverter` [IPropertyConverter\<TResult, TValue\>](../masstransit-initializers/ipropertyconverter-2)<br/>

## Methods

### **Convert\<T\>(InitializeContext\<T\>, TVariable)**

```csharp
public Task<TResult> Convert<T>(InitializeContext<T> context, TVariable input)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [InitializeContext\<T\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` TVariable<br/>

#### Returns

[Task\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
