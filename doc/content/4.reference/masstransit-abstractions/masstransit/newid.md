---

title: NewId

---

# NewId

Namespace: MassTransit

A NewId is a type that fits into the same space as a Guid/Uuid/unique identifier,
 but is guaranteed to be both unique and ordered, assuming it is generated using
 a single instance of the generator for each network address used.

```csharp
public struct NewId
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [NewId](../masstransit/newid)<br/>
Implements [IEquatable\<NewId\>](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1), [IComparable\<NewId\>](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable-1), [IComparable](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.iformattable)

## Fields

### **Empty**

```csharp
public static NewId Empty;
```

## Properties

### **Timestamp**

```csharp
public DateTime Timestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

## Constructors

### **NewId(Byte[])**

Creates a NewId using the specified byte array.

```csharp
public NewId(in Byte[] bytes)
```

#### Parameters

`bytes` [Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

### **NewId(String)**

```csharp
public NewId(in string value)
```

#### Parameters

`value` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **NewId(Int32, Int32, Int32, Int32)**

```csharp
public NewId(int a, int b, int c, int d)
```

#### Parameters

`a` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`b` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`c` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`d` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **NewId(Int32, Int16, Int16, Byte, Byte, Byte, Byte, Byte, Byte, Byte, Byte)**

```csharp
public NewId(int a, short b, short c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
```

#### Parameters

`a` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`b` [Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

`c` [Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

`d` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`e` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`f` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`g` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`h` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`i` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`j` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`k` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

## Methods

### **CompareTo(Object)**

```csharp
public int CompareTo(object obj)
```

#### Parameters

`obj` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **CompareTo(NewId)**

```csharp
public int CompareTo(NewId other)
```

#### Parameters

`other` [NewId](../masstransit/newid)<br/>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Equals(NewId)**

```csharp
public bool Equals(NewId other)
```

#### Parameters

`other` [NewId](../masstransit/newid)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ToString(String, IFormatProvider)**

```csharp
public string ToString(string format, IFormatProvider formatProvider)
```

#### Parameters

`format` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`formatProvider` [IFormatProvider](https://learn.microsoft.com/en-us/dotnet/api/system.iformatprovider)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ToString(INewIdFormatter, Boolean)**

```csharp
public string ToString(INewIdFormatter formatter, bool sequential)
```

#### Parameters

`formatter` [INewIdFormatter](../masstransit/inewidformatter)<br/>

`sequential` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ToGuid()**

```csharp
public Guid ToGuid()
```

#### Returns

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **ToSequentialGuid()**

```csharp
public Guid ToSequentialGuid()
```

#### Returns

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **FromGuid(Guid)**

```csharp
public static NewId FromGuid(in Guid guid)
```

#### Parameters

`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[NewId](../masstransit/newid)<br/>

### **FromSequentialGuid(Guid)**

```csharp
public static NewId FromSequentialGuid(in Guid guid)
```

#### Parameters

`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[NewId](../masstransit/newid)<br/>

### **ToByteArray()**

```csharp
public Byte[] ToByteArray()
```

#### Returns

[Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ToString(String)**

```csharp
public string ToString(string format)
```

#### Parameters

`format` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

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

### **SetGenerator(INewIdGenerator)**

```csharp
public static void SetGenerator(INewIdGenerator generator)
```

#### Parameters

`generator` [INewIdGenerator](../masstransit/inewidgenerator)<br/>

### **SetWorkerIdProvider(IWorkerIdProvider)**

```csharp
public static void SetWorkerIdProvider(IWorkerIdProvider provider)
```

#### Parameters

`provider` [IWorkerIdProvider](../masstransit/iworkeridprovider)<br/>

### **SetProcessIdProvider(IProcessIdProvider)**

```csharp
public static void SetProcessIdProvider(IProcessIdProvider provider)
```

#### Parameters

`provider` [IProcessIdProvider](../masstransit/iprocessidprovider)<br/>

### **SetTickProvider(ITickProvider)**

```csharp
public static void SetTickProvider(ITickProvider provider)
```

#### Parameters

`provider` [ITickProvider](../masstransit/itickprovider)<br/>

### **Next()**

Generate a NewId

```csharp
public static NewId Next()
```

#### Returns

[NewId](../masstransit/newid)<br/>

### **Next(Int32)**

Generate an array of NewIds

```csharp
public static NewId[] Next(int count)
```

#### Parameters

`count` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of NewIds to generate

#### Returns

[NewId[]](../masstransit/newid)<br/>

### **Next(NewId[], Int32, Int32)**

Generate an array of NewIds

```csharp
public static ArraySegment<NewId> Next(NewId[] ids, int index, int count)
```

#### Parameters

`ids` [NewId[]](../masstransit/newid)<br/>
An existing array

`index` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The starting offset for the newly generated ids

`count` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of NewIds to generate

#### Returns

[ArraySegment\<NewId\>](https://learn.microsoft.com/en-us/dotnet/api/system.arraysegment-1)<br/>

### **NextGuid(Int32)**

Generate an array of NewIds

```csharp
public static Guid[] NextGuid(int count)
```

#### Parameters

`count` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of NewIds to generate

#### Returns

[Guid[]](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **NextGuid(Guid[], Int32, Int32)**

Generate an array of NewIds

```csharp
public static ArraySegment<Guid> NextGuid(Guid[] ids, int index, int count)
```

#### Parameters

`ids` [Guid[]](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
An existing array

`index` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The starting offset for the newly generated ids

`count` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of NewIds to generate

#### Returns

[ArraySegment\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.arraysegment-1)<br/>

### **NextGuid()**

Generate a NewId, and return it as a Guid

```csharp
public static Guid NextGuid()
```

#### Returns

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **NextSequentialGuid()**

Generate a NewId, and return it as a Guid in sequential format

```csharp
public static Guid NextSequentialGuid()
```

#### Returns

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
