---

title: IWritePropertyCache<T>

---

# IWritePropertyCache\<T\>

Namespace: MassTransit.Internals

```csharp
public interface IWritePropertyCache<T>
```

#### Type Parameters

`T`<br/>

## Methods

### **CanWrite(String)**

```csharp
bool CanWrite(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetProperty\<TProperty\>(String)**

```csharp
IWriteProperty<T, TProperty> GetProperty<TProperty>(string name)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IWriteProperty\<T, TProperty\>](../masstransit-internals/iwriteproperty-2)<br/>

### **GetProperty\<TProperty\>(PropertyInfo)**

```csharp
IWriteProperty<T, TProperty> GetProperty<TProperty>(PropertyInfo propertyInfo)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

#### Returns

[IWriteProperty\<T, TProperty\>](../masstransit-internals/iwriteproperty-2)<br/>
