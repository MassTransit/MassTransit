---

title: CompositeFilter<T>

---

# CompositeFilter\<T\>

Namespace: MassTransit.Configuration

```csharp
public class CompositeFilter<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CompositeFilter\<T\>](../masstransit-configuration/compositefilter-1)

## Properties

### **Includes**

```csharp
public CompositePredicate<T> Includes { get; set; }
```

#### Property Value

[CompositePredicate\<T\>](../masstransit-configuration/compositepredicate-1)<br/>

### **Excludes**

```csharp
public CompositePredicate<T> Excludes { get; set; }
```

#### Property Value

[CompositePredicate\<T\>](../masstransit-configuration/compositepredicate-1)<br/>

## Constructors

### **CompositeFilter()**

```csharp
public CompositeFilter()
```

## Methods

### **Matches(T)**

```csharp
public bool Matches(T target)
```

#### Parameters

`target` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
