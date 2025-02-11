---

title: AddressEqualityComparer

---

# AddressEqualityComparer

Namespace: MassTransit.Transports

```csharp
public class AddressEqualityComparer : IEqualityComparer<Uri>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AddressEqualityComparer](../masstransit-transports/addressequalitycomparer)<br/>
Implements [IEqualityComparer\<Uri\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)

## Fields

### **Comparer**

```csharp
public static IEqualityComparer<Uri> Comparer;
```

## Constructors

### **AddressEqualityComparer()**

```csharp
public AddressEqualityComparer()
```

## Methods

### **Equals(Uri, Uri)**

```csharp
public bool Equals(Uri x, Uri y)
```

#### Parameters

`x` Uri<br/>

`y` Uri<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetHashCode(Uri)**

```csharp
public int GetHashCode(Uri obj)
```

#### Parameters

`obj` Uri<br/>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
