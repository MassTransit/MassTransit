---

title: StatePropertyConverter<TResult, TInstance>

---

# StatePropertyConverter\<TResult, TInstance\>

Namespace: MassTransit.Initializers.PropertyConverters

```csharp
public class StatePropertyConverter<TResult, TInstance> : IPropertyConverter<TResult, State<TInstance>>
```

#### Type Parameters

`TResult`<br/>

`TInstance`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StatePropertyConverter\<TResult, TInstance\>](../masstransit-initializers-propertyconverters/statepropertyconverter-2)<br/>
Implements [IPropertyConverter\<TResult, State\<TInstance\>\>](../masstransit-initializers/ipropertyconverter-2)

## Constructors

### **StatePropertyConverter(IPropertyConverter\<TResult, String\>)**

```csharp
public StatePropertyConverter(IPropertyConverter<TResult, string> propertyConverter)
```

#### Parameters

`propertyConverter` [IPropertyConverter\<TResult, String\>](../masstransit-initializers/ipropertyconverter-2)<br/>

## Methods

### **Convert\<T\>(InitializeContext\<T\>, State\<TInstance\>)**

```csharp
public Task<TResult> Convert<T>(InitializeContext<T> context, State<TInstance> input)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [InitializeContext\<T\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` [State\<TInstance\>](../../masstransit-abstractions/masstransit/state-1)<br/>

#### Returns

[Task\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
