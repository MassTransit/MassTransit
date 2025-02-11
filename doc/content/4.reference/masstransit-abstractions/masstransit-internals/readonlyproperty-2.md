---

title: ReadOnlyProperty<T, TProperty>

---

# ReadOnlyProperty\<T, TProperty\>

Namespace: MassTransit.Internals

```csharp
public class ReadOnlyProperty<T, TProperty>
```

#### Type Parameters

`T`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReadOnlyProperty\<T, TProperty\>](../masstransit-internals/readonlyproperty-2)

## Fields

### **GetProperty**

```csharp
public Func<T, TProperty> GetProperty;
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
public TProperty Get(T instance)
```

#### Parameters

`instance` T<br/>

#### Returns

TProperty<br/>
