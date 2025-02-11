---

title: FutureSubscription

---

# FutureSubscription

Namespace: MassTransit

```csharp
public class FutureSubscription : IEquatable<FutureSubscription>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureSubscription](../masstransit/futuresubscription)<br/>
Implements [IEquatable\<FutureSubscription\>](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **Comparer**

```csharp
public static IEqualityComparer<FutureSubscription> Comparer { get; }
```

#### Property Value

[IEqualityComparer\<FutureSubscription\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)<br/>

### **RequestId**

```csharp
public Nullable<Guid> RequestId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Address**

```csharp
public Uri Address { get; }
```

#### Property Value

Uri<br/>

## Constructors

### **FutureSubscription(Uri, Nullable\<Guid\>)**

```csharp
public FutureSubscription(Uri address, Nullable<Guid> requestId)
```

#### Parameters

`address` Uri<br/>

`requestId` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Methods

### **Equals(FutureSubscription)**

```csharp
public bool Equals(FutureSubscription other)
```

#### Parameters

`other` [FutureSubscription](../masstransit/futuresubscription)<br/>

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
