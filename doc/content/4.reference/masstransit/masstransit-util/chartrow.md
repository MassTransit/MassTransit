---

title: ChartRow

---

# ChartRow

Namespace: MassTransit.Util

```csharp
public class ChartRow
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ChartRow](../masstransit-util/chartrow)

## Properties

### **Title**

```csharp
public string Title { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Duration**

```csharp
public string Duration { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Timeline**

```csharp
public string Timeline { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **ChartRow(String, String, String, Object[])**

```csharp
public ChartRow(string title, string duration, string timeline, Object[] columns)
```

#### Parameters

`title` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`duration` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`timeline` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`columns` [Object[]](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

## Methods

### **GetColumn(Int32)**

```csharp
public object GetColumn(int column)
```

#### Parameters

`column` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
