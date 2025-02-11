---

title: ChartTable

---

# ChartTable

Namespace: MassTransit.Util

```csharp
public class ChartTable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ChartTable](../masstransit-util/charttable)

## Constructors

### **ChartTable(Int32)**

```csharp
public ChartTable(int chartWidth)
```

#### Parameters

`chartWidth` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **Add(String, DateTime, Nullable\<TimeSpan\>, Object[])**

```csharp
public ChartTable Add(string text, DateTime startTime, Nullable<TimeSpan> duration, Object[] columns)
```

#### Parameters

`text` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`startTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`duration` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`columns` [Object[]](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[ChartTable](../masstransit-util/charttable)<br/>

### **GetRows()**

```csharp
public IEnumerable<ChartRow> GetRows()
```

#### Returns

[IEnumerable\<ChartRow\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **CalculateRange()**

```csharp
public ValueTuple<DateTime, DateTime> CalculateRange()
```

#### Returns

[ValueTuple\<DateTime, DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple-2)<br/>
