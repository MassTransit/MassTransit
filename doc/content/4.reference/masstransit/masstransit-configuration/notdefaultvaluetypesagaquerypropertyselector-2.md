---

title: NotDefaultValueTypeSagaQueryPropertySelector<TData, TProperty>

---

# NotDefaultValueTypeSagaQueryPropertySelector\<TData, TProperty\>

Namespace: MassTransit.Configuration

```csharp
public class NotDefaultValueTypeSagaQueryPropertySelector<TData, TProperty> : ISagaQueryPropertySelector<TData, TProperty>
```

#### Type Parameters

`TData`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [NotDefaultValueTypeSagaQueryPropertySelector\<TData, TProperty\>](../masstransit-configuration/notdefaultvaluetypesagaquerypropertyselector-2)<br/>
Implements [ISagaQueryPropertySelector\<TData, TProperty\>](../masstransit-configuration/isagaquerypropertyselector-2)

## Constructors

### **NotDefaultValueTypeSagaQueryPropertySelector(Func\<ConsumeContext\<TData\>, TProperty\>)**

```csharp
public NotDefaultValueTypeSagaQueryPropertySelector(Func<ConsumeContext<TData>, TProperty> selector)
```

#### Parameters

`selector` [Func\<ConsumeContext\<TData\>, TProperty\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

## Methods

### **TryGetProperty(ConsumeContext\<TData\>, TProperty)**

```csharp
public bool TryGetProperty(ConsumeContext<TData> context, out TProperty property)
```

#### Parameters

`context` [ConsumeContext\<TData\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`property` TProperty<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
