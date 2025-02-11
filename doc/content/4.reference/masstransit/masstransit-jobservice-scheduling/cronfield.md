---

title: CronField

---

# CronField

Namespace: MassTransit.JobService.Scheduling

```csharp
public sealed class CronField : IEnumerable<Int32>, IEnumerable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CronField](../masstransit-jobservice-scheduling/cronfield)<br/>
Implements [IEnumerable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **CronField()**

```csharp
public CronField()
```

## Methods

### **GetEnumerator()**

```csharp
public IEnumerator<int> GetEnumerator()
```

#### Returns

[IEnumerator\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1)<br/>

### **Clear()**

```csharp
internal void Clear()
```

### **TryGetMinValueStartingFrom(Int32, Int32)**

```csharp
internal bool TryGetMinValueStartingFrom(int start, out int min)
```

#### Parameters

`start` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`min` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Add(Int32)**

```csharp
public void Add(int value)
```

#### Parameters

`value` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Contains(Int32)**

```csharp
public bool Contains(int value)
```

#### Parameters

`value` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
