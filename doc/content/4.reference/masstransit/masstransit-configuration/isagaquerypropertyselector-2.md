---

title: ISagaQueryPropertySelector<TData, TProperty>

---

# ISagaQueryPropertySelector\<TData, TProperty\>

Namespace: MassTransit.Configuration

```csharp
public interface ISagaQueryPropertySelector<TData, TProperty>
```

#### Type Parameters

`TData`<br/>

`TProperty`<br/>

## Methods

### **TryGetProperty(ConsumeContext\<TData\>, TProperty)**

```csharp
bool TryGetProperty(ConsumeContext<TData> context, out TProperty property)
```

#### Parameters

`context` [ConsumeContext\<TData\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`property` TProperty<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
