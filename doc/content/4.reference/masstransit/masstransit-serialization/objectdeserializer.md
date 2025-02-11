---

title: ObjectDeserializer

---

# ObjectDeserializer

Namespace: MassTransit.Serialization

```csharp
public static class ObjectDeserializer
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ObjectDeserializer](../masstransit-serialization/objectdeserializer)

## Properties

### **Default**

```csharp
public static IObjectDeserializer Default { set; }
```

#### Property Value

[IObjectDeserializer](../../masstransit-abstractions/masstransit/iobjectdeserializer)<br/>

### **Current**

```csharp
public static IObjectDeserializer Current { set; }
```

#### Property Value

[IObjectDeserializer](../../masstransit-abstractions/masstransit/iobjectdeserializer)<br/>

## Methods

### **Serialize(Object)**

```csharp
public static string Serialize(object value)
```

#### Parameters

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Deserialize\<T\>(Object, T)**

```csharp
public static T Deserialize<T>(object value, T defaultValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`defaultValue` T<br/>

#### Returns

T<br/>

### **Deserialize\<T\>(Object, Nullable\<T\>)**

```csharp
public static Nullable<T> Deserialize<T>(object value, Nullable<T> defaultValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`defaultValue` [Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
