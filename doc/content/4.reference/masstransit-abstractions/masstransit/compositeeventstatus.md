---

title: CompositeEventStatus

---

# CompositeEventStatus

Namespace: MassTransit

```csharp
public struct CompositeEventStatus
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [CompositeEventStatus](../masstransit/compositeeventstatus)<br/>
Implements [IComparable\<CompositeEventStatus\>](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable-1)

## Properties

### **Status**

```csharp
public string Status { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Bits**

```csharp
public int Bits { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **CompositeEventStatus(Int32)**

```csharp
public CompositeEventStatus(int bits)
```

#### Parameters

`bits` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **CompareTo(CompositeEventStatus)**

```csharp
public int CompareTo(CompositeEventStatus other)
```

#### Parameters

`other` [CompositeEventStatus](../masstransit/compositeeventstatus)<br/>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Equals(CompositeEventStatus)**

```csharp
public bool Equals(CompositeEventStatus other)
```

#### Parameters

`other` [CompositeEventStatus](../masstransit/compositeeventstatus)<br/>

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

### **Set(Int32)**

```csharp
public void Set(int flag)
```

#### Parameters

`flag` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **IsSet(Int32)**

```csharp
public bool IsSet(int flag)
```

#### Parameters

`flag` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
