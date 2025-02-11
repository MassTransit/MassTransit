---

title: FilterSet<T>

---

# FilterSet\<T\>

Namespace: MassTransit.Testing

```csharp
public class FilterSet<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FilterSet\<T\>](../masstransit-testing/filterset-1)

## Methods

### **Add(FilterDelegate\<T\>)**

```csharp
protected FilterSet<T> Add(FilterDelegate<T> filter)
```

#### Parameters

`filter` [FilterDelegate\<T\>](../masstransit-testing/filterdelegate-1)<br/>

#### Returns

[FilterSet\<T\>](../masstransit-testing/filterset-1)<br/>

### **All(T)**

```csharp
public bool All(T target)
```

#### Parameters

`target` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Any(T)**

```csharp
public bool Any(T target)
```

#### Parameters

`target` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **NotAny(T)**

```csharp
public bool NotAny(T target)
```

#### Parameters

`target` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **None(T)**

```csharp
public bool None(T target)
```

#### Parameters

`target` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
