---

title: ReadWriteProperty

---

# ReadWriteProperty

Namespace: MassTransit.Internals

```csharp
public class ReadWriteProperty : ReadOnlyProperty
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReadOnlyProperty](../masstransit-internals/readonlyproperty) → [ReadWriteProperty](../masstransit-internals/readwriteproperty)

## Fields

### **SetProperty**

```csharp
public Action<object, object> SetProperty;
```

### **GetProperty**

```csharp
public Func<object, object> GetProperty;
```

## Properties

### **Property**

```csharp
public PropertyInfo Property { get; }
```

#### Property Value

[PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

## Constructors

### **ReadWriteProperty(PropertyInfo)**

```csharp
public ReadWriteProperty(PropertyInfo property)
```

#### Parameters

`property` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

## Methods

### **Set(Object, Object)**

```csharp
public void Set(object instance, object value)
```

#### Parameters

`instance` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
