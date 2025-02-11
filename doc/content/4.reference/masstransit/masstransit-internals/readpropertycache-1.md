---

title: ReadPropertyCache<T>

---

# ReadPropertyCache\<T\>

Namespace: MassTransit.Internals

```csharp
public class ReadPropertyCache<T> : IReadPropertyCache<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReadPropertyCache\<T\>](../masstransit-internals/readpropertycache-1)<br/>
Implements [IReadPropertyCache\<T\>](../masstransit-internals/ireadpropertycache-1)

## Methods

### **GetProperty\<TProperty\>(String)**

```csharp
public static IReadProperty<T, TProperty> GetProperty<TProperty>(string name)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IReadProperty\<T, TProperty\>](../masstransit-internals/ireadproperty-2)<br/>

### **TryGetProperty\<TProperty\>(String, IReadProperty\<T, TProperty\>)**

```csharp
public static bool TryGetProperty<TProperty>(string name, out IReadProperty<T, TProperty> property)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`property` [IReadProperty\<T, TProperty\>](../masstransit-internals/ireadproperty-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetProperty\<TProperty\>(PropertyInfo)**

```csharp
public static IReadProperty<T, TProperty> GetProperty<TProperty>(PropertyInfo propertyInfo)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

#### Returns

[IReadProperty\<T, TProperty\>](../masstransit-internals/ireadproperty-2)<br/>
