---

title: Bind<TKey, TValue>

---

# Bind\<TKey, TValue\>

Namespace: MassTransit.DependencyInjection

Bind is used to store types bound to their owner, such as an IBusControl to an IMyBus.

```csharp
public class Bind<TKey, TValue> : IEquatable<Bind<TKey, TValue>>
```

#### Type Parameters

`TKey`<br/>
The key type

`TValue`<br/>
The bound type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Bind\<TKey, TValue\>](../masstransit-dependencyinjection/bind-2)<br/>
Implements [IEquatable\<Bind\<TKey, TValue\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **Value**

```csharp
public TValue Value { get; }
```

#### Property Value

TValue<br/>

## Constructors

### **Bind(TValue)**

```csharp
public Bind(TValue value)
```

#### Parameters

`value` TValue<br/>

## Methods

### **Equals(Bind\<TKey, TValue\>)**

```csharp
public bool Equals(Bind<TKey, TValue> other)
```

#### Parameters

`other` [Bind\<TKey, TValue\>](../masstransit-dependencyinjection/bind-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Equals(Object)**

```csharp
public bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetHashCode()**

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Create\<T\>(T)**

```csharp
public static Bind<TKey, TValue, T> Create<T>(T value)
```

#### Type Parameters

`T`<br/>

#### Parameters

`value` T<br/>

#### Returns

[Bind\<TKey, TValue, T\>](../masstransit-dependencyinjection/bind-3)<br/>
