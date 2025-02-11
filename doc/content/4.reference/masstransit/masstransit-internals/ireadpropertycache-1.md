---

title: IReadPropertyCache<T>

---

# IReadPropertyCache\<T\>

Namespace: MassTransit.Internals

```csharp
public interface IReadPropertyCache<T>
```

#### Type Parameters

`T`<br/>

## Methods

### **GetProperty\<TProperty\>(String)**

```csharp
IReadProperty<T, TProperty> GetProperty<TProperty>(string name)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IReadProperty\<T, TProperty\>](../masstransit-internals/ireadproperty-2)<br/>

### **GetProperty\<TProperty\>(PropertyInfo)**

```csharp
IReadProperty<T, TProperty> GetProperty<TProperty>(PropertyInfo propertyInfo)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

#### Returns

[IReadProperty\<T, TProperty\>](../masstransit-internals/ireadproperty-2)<br/>

### **TryGetProperty\<TProperty\>(String, IReadProperty\<T, TProperty\>)**

```csharp
bool TryGetProperty<TProperty>(string name, out IReadProperty<T, TProperty> property)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`property` [IReadProperty\<T, TProperty\>](../masstransit-internals/ireadproperty-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
