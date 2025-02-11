---

title: ReadOnlyProperty<T>

---

# ReadOnlyProperty\<T\>

Namespace: MassTransit.Internals

```csharp
public class ReadOnlyProperty<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReadOnlyProperty\<T\>](../masstransit-internals/readonlyproperty-1)

## Fields

### **GetProperty**

```csharp
public Func<T, object> GetProperty;
```

## Properties

### **Property**

```csharp
public PropertyInfo Property { get; private set; }
```

#### Property Value

[PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

## Constructors

### **ReadOnlyProperty(Expression\<Func\<T, Object\>\>)**

```csharp
public ReadOnlyProperty(Expression<Func<T, object>> propertyExpression)
```

#### Parameters

`propertyExpression` Expression\<Func\<T, Object\>\><br/>

### **ReadOnlyProperty(PropertyInfo)**

```csharp
public ReadOnlyProperty(PropertyInfo property)
```

#### Parameters

`property` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

## Methods

### **Get(T)**

```csharp
public object Get(T instance)
```

#### Parameters

`instance` T<br/>

#### Returns

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
