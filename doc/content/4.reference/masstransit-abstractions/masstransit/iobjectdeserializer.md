---

title: IObjectDeserializer

---

# IObjectDeserializer

Namespace: MassTransit

```csharp
public interface IObjectDeserializer
```

## Methods

### **DeserializeObject\<T\>(Object, T)**

```csharp
T DeserializeObject<T>(object value, T defaultValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`defaultValue` T<br/>

#### Returns

T<br/>

### **DeserializeObject\<T\>(Object, Nullable\<T\>)**

```csharp
Nullable<T> DeserializeObject<T>(object value, Nullable<T> defaultValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`defaultValue` [Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SerializeObject(Object)**

Serialize the dictionary to a message body, using the underlying serializer to ensure objects are properly serialized.

```csharp
MessageBody SerializeObject(object value)
```

#### Parameters

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[MessageBody](../masstransit/messagebody)<br/>
