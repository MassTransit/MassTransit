---

title: TextTable

---

# TextTable

Namespace: MassTransit.Util

Generates a monospaced text table, useful in trace output formats. Shamelessly inspired by ConsoleTables
 https://github.com/khalidabuhakmeh/ConsoleTables

```csharp
public class TextTable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TextTable](../masstransit-util/texttable)

## Properties

### **Options**

```csharp
public TextTableOptions Options { get; }
```

#### Property Value

[TextTableOptions](../masstransit-util/texttableoptions)<br/>

## Constructors

### **TextTable(String[])**

```csharp
public TextTable(String[] columns)
```

#### Parameters

`columns` [String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **TextTable(TextTableOptions)**

```csharp
public TextTable(TextTableOptions options)
```

#### Parameters

`options` [TextTableOptions](../masstransit-util/texttableoptions)<br/>

## Methods

### **AddColumns(String[])**

```csharp
public TextTable AddColumns(String[] names)
```

#### Parameters

`names` [String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[TextTable](../masstransit-util/texttable)<br/>

### **AddColumns(IEnumerable\<String\>)**

```csharp
public TextTable AddColumns(IEnumerable<string> names)
```

#### Parameters

`names` [IEnumerable\<String\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

#### Returns

[TextTable](../masstransit-util/texttable)<br/>

### **AddRow(Object[])**

```csharp
public TextTable AddRow(Object[] values)
```

#### Parameters

`values` [Object[]](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[TextTable](../masstransit-util/texttable)<br/>

### **Configure(Action\<TextTableOptions\>)**

```csharp
public TextTable Configure(Action<TextTableOptions> action)
```

#### Parameters

`action` [Action\<TextTableOptions\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[TextTable](../masstransit-util/texttable)<br/>

### **Create\<T\>(IEnumerable\<T\>)**

Create a table from an existing enumerable collection

```csharp
public static TextTable Create<T>(IEnumerable<T> rows)
```

#### Type Parameters

`T`<br/>
The collection element type

#### Parameters

`rows` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
The collection

#### Returns

[TextTable](../masstransit-util/texttable)<br/>

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Write()**

```csharp
public void Write()
```

### **SetColumn(Int32, String, Type)**

```csharp
public TextTable SetColumn(int column, string name, Type columnType)
```

#### Parameters

`column` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`columnType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[TextTable](../masstransit-util/texttable)<br/>

### **HideRowSeparator()**

```csharp
public TextTable HideRowSeparator()
```

#### Returns

[TextTable](../masstransit-util/texttable)<br/>

### **EnableCount(Boolean)**

```csharp
public TextTable EnableCount(bool enabled)
```

#### Parameters

`enabled` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[TextTable](../masstransit-util/texttable)<br/>

### **SetRightNumberAlignment()**

```csharp
public TextTable SetRightNumberAlignment()
```

#### Returns

[TextTable](../masstransit-util/texttable)<br/>

### **OutputTo(TextWriter)**

```csharp
public TextTable OutputTo(TextWriter textWriter)
```

#### Parameters

`textWriter` [TextWriter](https://learn.microsoft.com/en-us/dotnet/api/system.io.textwriter)<br/>

#### Returns

[TextTable](../masstransit-util/texttable)<br/>
