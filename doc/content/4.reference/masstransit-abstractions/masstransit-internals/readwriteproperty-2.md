---

title: ReadWriteProperty<T, TProperty>

---

# ReadWriteProperty\<T, TProperty\>

Namespace: MassTransit.Internals

```csharp
public class ReadWriteProperty<T, TProperty> : ReadOnlyProperty<T, TProperty>
```

#### Type Parameters

`T`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReadOnlyProperty\<T, TProperty\>](../masstransit-internals/readonlyproperty-2) → [ReadWriteProperty\<T, TProperty\>](../masstransit-internals/readwriteproperty-2)

## Fields

### **SetProperty**

```csharp
public Action<T, TProperty> SetProperty;
```

### **GetProperty**

```csharp
public Func<T, TProperty> GetProperty;
```

## Properties

### **Property**

```csharp
public PropertyInfo Property { get; }
```

#### Property Value

[PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

## Constructors

### **ReadWriteProperty(Expression\<Func\<T, Object\>\>)**

```csharp
public ReadWriteProperty(Expression<Func<T, object>> propertyExpression)
```

#### Parameters

`propertyExpression` Expression\<Func\<T, Object\>\><br/>

### **ReadWriteProperty(Expression\<Func\<T, Object\>\>, Boolean)**

```csharp
public ReadWriteProperty(Expression<Func<T, object>> propertyExpression, bool includeNonPublic)
```

#### Parameters

`propertyExpression` Expression\<Func\<T, Object\>\><br/>

`includeNonPublic` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ReadWriteProperty(PropertyInfo)**

```csharp
public ReadWriteProperty(PropertyInfo property)
```

#### Parameters

`property` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

## Methods

### **Set(T, TProperty)**

```csharp
public void Set(T instance, TProperty value)
```

#### Parameters

`instance` T<br/>

`value` TProperty<br/>
