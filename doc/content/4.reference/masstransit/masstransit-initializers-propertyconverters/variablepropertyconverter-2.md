---

title: VariablePropertyConverter<TResult, TVariable>

---

# VariablePropertyConverter\<TResult, TVariable\>

Namespace: MassTransit.Initializers.PropertyConverters

```csharp
public class VariablePropertyConverter<TResult, TVariable> : IPropertyConverter<TResult, TVariable>
```

#### Type Parameters

`TResult`<br/>

`TVariable`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [VariablePropertyConverter\<TResult, TVariable\>](../masstransit-initializers-propertyconverters/variablepropertyconverter-2)<br/>
Implements [IPropertyConverter\<TResult, TVariable\>](../masstransit-initializers/ipropertyconverter-2)

## Constructors

### **VariablePropertyConverter()**

```csharp
public VariablePropertyConverter()
```

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
