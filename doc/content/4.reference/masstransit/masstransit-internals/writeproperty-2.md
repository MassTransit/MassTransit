---

title: WriteProperty<T, TProperty>

---

# WriteProperty\<T, TProperty\>

Namespace: MassTransit.Internals

```csharp
public class WriteProperty<T, TProperty> : IWriteProperty<T, TProperty>, IWriteProperty<T>
```

#### Type Parameters

`T`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [WriteProperty\<T, TProperty\>](../masstransit-internals/writeproperty-2)<br/>
Implements [IWriteProperty\<T, TProperty\>](../masstransit-internals/iwriteproperty-2), [IWriteProperty\<T\>](../masstransit-internals/iwriteproperty-1)

## Properties

### **TargetType**

```csharp
public Type TargetType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

## Constructors

### **WriteProperty(Type, PropertyInfo)**

```csharp
public WriteProperty(Type implementationType, PropertyInfo propertyInfo)
```

#### Parameters

`implementationType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

## Methods

### **Set(T, TProperty)**

```csharp
public void Set(T content, TProperty value)
```

#### Parameters

`content` T<br/>

`value` TProperty<br/>
