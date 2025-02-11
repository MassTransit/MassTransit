---

title: StatePropertyConverter<TInstance>

---

# StatePropertyConverter\<TInstance\>

Namespace: MassTransit.Initializers.PropertyConverters

```csharp
public class StatePropertyConverter<TInstance> : IPropertyConverter<String, State<TInstance>>
```

#### Type Parameters

`TInstance`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StatePropertyConverter\<TInstance\>](../masstransit-initializers-propertyconverters/statepropertyconverter-1)<br/>
Implements [IPropertyConverter\<String, State\<TInstance\>\>](../masstransit-initializers/ipropertyconverter-2)

## Constructors

### **StatePropertyConverter()**

```csharp
public StatePropertyConverter()
```

## Methods

### **Convert\<T\>(InitializeContext\<T\>, State\<TInstance\>)**

```csharp
public Task<string> Convert<T>(InitializeContext<T> context, State<TInstance> input)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [InitializeContext\<T\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` [State\<TInstance\>](../../masstransit-abstractions/masstransit/state-1)<br/>

#### Returns

[Task\<String\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
