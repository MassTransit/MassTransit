---

title: WritePropertyCache<T>

---

# WritePropertyCache\<T\>

Namespace: MassTransit.Internals

```csharp
public class WritePropertyCache<T> : IWritePropertyCache<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [WritePropertyCache\<T\>](../masstransit-internals/writepropertycache-1)<br/>
Implements [IWritePropertyCache\<T\>](../masstransit-internals/iwritepropertycache-1)

## Methods

### **GetProperty\<TProperty\>(String)**

```csharp
public static IWriteProperty<T, TProperty> GetProperty<TProperty>(string name)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IWriteProperty\<T, TProperty\>](../masstransit-internals/iwriteproperty-2)<br/>

### **GetProperty\<TProperty\>(PropertyInfo)**

```csharp
public static IWriteProperty<T, TProperty> GetProperty<TProperty>(PropertyInfo propertyInfo)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

#### Returns

[IWriteProperty\<T, TProperty\>](../masstransit-internals/iwriteproperty-2)<br/>

### **CanWrite(String)**

```csharp
public static bool CanWrite(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
