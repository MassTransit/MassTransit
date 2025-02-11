---

title: CompositePredicate<T>

---

# CompositePredicate\<T\>

Namespace: MassTransit.Configuration

```csharp
public class CompositePredicate<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CompositePredicate\<T\>](../masstransit-configuration/compositepredicate-1)

## Constructors

### **CompositePredicate()**

```csharp
public CompositePredicate()
```

## Methods

### **Add(Func\<T, Boolean\>)**

```csharp
public void Add(Func<T, bool> filter)
```

#### Parameters

`filter` [Func\<T, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **MatchesAll(T)**

```csharp
public bool MatchesAll(T target)
```

#### Parameters

`target` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **MatchesAny(T)**

```csharp
public bool MatchesAny(T target)
```

#### Parameters

`target` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **MatchesNone(T)**

```csharp
public bool MatchesNone(T target)
```

#### Parameters

`target` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **DoesNotMatcheAny(T)**

```csharp
public bool DoesNotMatcheAny(T target)
```

#### Parameters

`target` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
