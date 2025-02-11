---

title: TextTableOptions

---

# TextTableOptions

Namespace: MassTransit.Util

```csharp
public class TextTableOptions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TextTableOptions](../masstransit-util/texttableoptions)

## Properties

### **Columns**

The column names

```csharp
public IEnumerable<string> Columns { get; set; }
```

#### Property Value

[IEnumerable\<String\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **EnableCount**

Include the row count at the end of the table

```csharp
public bool EnableCount { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **NumberAlignment**

Specify the number alignment (defaults to left)

```csharp
public NumberAlignment NumberAlignment { get; set; }
```

#### Property Value

[NumberAlignment](../masstransit-util/numberalignment)<br/>

### **Out**

The [TextWriter](https://learn.microsoft.com/en-us/dotnet/api/system.io.textwriter) to write to. Defaults to .

```csharp
public TextWriter Out { get; set; }
```

#### Property Value

[TextWriter](https://learn.microsoft.com/en-us/dotnet/api/system.io.textwriter)<br/>

### **ShowRowSeparator**

```csharp
public bool ShowRowSeparator { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **TextTableOptions()**

```csharp
public TextTableOptions()
```
