---

title: FutureLocation

---

# FutureLocation

Namespace: MassTransit

```csharp
public struct FutureLocation
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [FutureLocation](../masstransit/futurelocation)

## Fields

### **Address**

```csharp
public Uri Address;
```

### **Id**

```csharp
public Guid Id;
```

## Constructors

### **FutureLocation(Uri)**

```csharp
public FutureLocation(Uri location)
```

#### Parameters

`location` Uri<br/>

### **FutureLocation(Guid, Uri)**

```csharp
public FutureLocation(Guid id, Uri address)
```

#### Parameters

`id` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`address` Uri<br/>
