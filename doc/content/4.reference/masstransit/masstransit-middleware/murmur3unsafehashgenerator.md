---

title: Murmur3UnsafeHashGenerator

---

# Murmur3UnsafeHashGenerator

Namespace: MassTransit.Middleware

```csharp
public class Murmur3UnsafeHashGenerator : IHashGenerator
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Murmur3UnsafeHashGenerator](../masstransit-middleware/murmur3unsafehashgenerator)<br/>
Implements [IHashGenerator](../masstransit-middleware/ihashgenerator)

## Constructors

### **Murmur3UnsafeHashGenerator()**

```csharp
public Murmur3UnsafeHashGenerator()
```

## Methods

### **Hash(Byte[])**

```csharp
public uint Hash(Byte[] data)
```

#### Parameters

`data` [Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

#### Returns

[UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br/>

### **Hash(String)**

```csharp
public uint Hash(string s)
```

#### Parameters

`s` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br/>

### **Hash(Byte[], Int32, UInt32, UInt32)**

```csharp
public uint Hash(Byte[] data, int offset, uint count, uint seed)
```

#### Parameters

`data` [Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`offset` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`count` [UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br/>

`seed` [UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br/>

#### Returns

[UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br/>
