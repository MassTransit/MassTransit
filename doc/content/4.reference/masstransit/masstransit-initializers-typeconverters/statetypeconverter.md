---

title: StateTypeConverter

---

# StateTypeConverter

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class StateTypeConverter : ITypeConverter<String, State>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StateTypeConverter](../masstransit-initializers-typeconverters/statetypeconverter)<br/>
Implements [ITypeConverter\<String, State\>](../masstransit-initializers/itypeconverter-2)

## Constructors

### **StateTypeConverter()**

```csharp
public StateTypeConverter()
```

## Methods

### **TryConvert(State, String)**

```csharp
public bool TryConvert(State input, out string result)
```

#### Parameters

`input` [State](../../masstransit-abstractions/masstransit/state)<br/>

`result` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
