---

title: HasValueTypeSagaQueryPropertySelector<TData, TProperty>

---

# HasValueTypeSagaQueryPropertySelector\<TData, TProperty\>

Namespace: MassTransit.Configuration

```csharp
public class HasValueTypeSagaQueryPropertySelector<TData, TProperty> : ISagaQueryPropertySelector<TData, Nullable<TProperty>>
```

#### Type Parameters

`TData`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [HasValueTypeSagaQueryPropertySelector\<TData, TProperty\>](../masstransit-configuration/hasvaluetypesagaquerypropertyselector-2)<br/>
Implements [ISagaQueryPropertySelector\<TData, Nullable\<TProperty\>\>](../masstransit-configuration/isagaquerypropertyselector-2)

## Constructors

### **HasValueTypeSagaQueryPropertySelector(Func\<ConsumeContext\<TData\>, Nullable\<TProperty\>\>)**

```csharp
public HasValueTypeSagaQueryPropertySelector(Func<ConsumeContext<TData>, Nullable<TProperty>> selector)
```

#### Parameters

`selector` [Func\<ConsumeContext\<TData\>, Nullable\<TProperty\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

## Methods

### **TryGetProperty(ConsumeContext\<TData\>, Nullable\<TProperty\>)**

```csharp
public bool TryGetProperty(ConsumeContext<TData> context, out Nullable<TProperty> property)
```

#### Parameters

`context` [ConsumeContext\<TData\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`property` [Nullable\<TProperty\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
