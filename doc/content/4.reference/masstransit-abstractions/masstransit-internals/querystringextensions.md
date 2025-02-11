---

title: QueryStringExtensions

---

# QueryStringExtensions

Namespace: MassTransit.Internals

```csharp
public static class QueryStringExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [QueryStringExtensions](../masstransit-internals/querystringextensions)

## Methods

### **TryGetValueFromQueryString(Uri, String, String)**

```csharp
public static bool TryGetValueFromQueryString(Uri uri, string key, out string value)
```

#### Parameters

`uri` Uri<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetValueFromQueryString\<T\>(Uri, String, T)**

```csharp
public static T GetValueFromQueryString<T>(Uri uri, string key, T defaultValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`uri` Uri<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`defaultValue` T<br/>

#### Returns

T<br/>

### **ParseHostPath(Uri)**

Parse the host path, which on a host address might be a virtual host, a scope, etc.

```csharp
public static string ParseHostPath(Uri address)
```

#### Parameters

`address` Uri<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ParseHostPathAndEntityName(Uri, String, String)**

Parse the host path and entity name from the address

```csharp
public static void ParseHostPathAndEntityName(Uri address, out string hostPath, out string entityName)
```

#### Parameters

`address` Uri<br/>

`hostPath` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`entityName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **SplitQueryString(Uri)**

Split the query string into an enumerable stream of tuples

```csharp
public static IEnumerable<ValueTuple<string, string>> SplitQueryString(Uri address)
```

#### Parameters

`address` Uri<br/>

#### Returns

[IEnumerable\<ValueTuple\<String, String\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
