---

title: ReadWriteProperty<T>

---

# ReadWriteProperty\<T\>

Namespace: MassTransit.Internals

```csharp
public class ReadWriteProperty<T> : ReadOnlyProperty<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReadOnlyProperty\<T\>](../masstransit-internals/readonlyproperty-1) → [ReadWriteProperty\<T\>](../masstransit-internals/readwriteproperty-1)

## Fields

### **SetProperty**

```csharp
public Action<T, object> SetProperty;
```

### **GetProperty**

```csharp
public Func<T, object> GetProperty;
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

### **Set(T, Object)**

```csharp
public void Set(T instance, object value)
```

#### Parameters

`instance` T<br/>

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
