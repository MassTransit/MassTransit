---

title: ExceptionUtil

---

# ExceptionUtil

Namespace: MassTransit.Util

```csharp
public static class ExceptionUtil
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExceptionUtil](../masstransit-util/exceptionutil)

## Methods

### **GetMessage(Exception)**

```csharp
public static string GetMessage(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **GetStackTrace(Exception)**

```csharp
public static string GetStackTrace(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **GetExceptionHeaderDictionary(Exception)**

```csharp
public static IDictionary<string, object> GetExceptionHeaderDictionary(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

### **GetExceptionHeaderDetail(Exception)**

```csharp
public static ValueTuple<Dictionary<string, object>, string> GetExceptionHeaderDetail(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[ValueTuple\<Dictionary\<String, Object\>, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple-2)<br/>
