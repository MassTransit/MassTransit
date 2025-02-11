---

title: ReadOnlyProperty

---

# ReadOnlyProperty

Namespace: MassTransit.Internals

```csharp
public class ReadOnlyProperty
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReadOnlyProperty](../masstransit-internals/readonlyproperty)

## Fields

### **GetProperty**

```csharp
public Func<object, object> GetProperty;
```

## Properties

### **Property**

```csharp
public PropertyInfo Property { get; private set; }
```

#### Property Value

[PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

## Constructors

### **ReadOnlyProperty(PropertyInfo)**

```csharp
public ReadOnlyProperty(PropertyInfo property)
```

#### Parameters

`property` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

## Methods

### **Get(Object)**

```csharp
public object Get(object instance)
```

#### Parameters

`instance` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
