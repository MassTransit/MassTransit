---

title: ITypeCache<T>

---

# ITypeCache\<T\>

Namespace: MassTransit.Internals

```csharp
public interface ITypeCache<T>
```

#### Type Parameters

`T`<br/>

## Properties

### **ShortName**

```csharp
public abstract string ShortName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ReadOnlyPropertyCache**

```csharp
public abstract IReadOnlyPropertyCache<T> ReadOnlyPropertyCache { get; }
```

#### Property Value

[IReadOnlyPropertyCache\<T\>](../masstransit-internals/ireadonlypropertycache-1)<br/>

### **ReadWritePropertyCache**

```csharp
public abstract IReadWritePropertyCache<T> ReadWritePropertyCache { get; }
```

#### Property Value

[IReadWritePropertyCache\<T\>](../masstransit-internals/ireadwritepropertycache-1)<br/>
