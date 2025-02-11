---

title: JsonMessageBody

---

# JsonMessageBody

Namespace: MassTransit

If the incoming message is in a JSON format, use this to unwrap the JSON document from any transport-specific encapsulation

```csharp
public interface JsonMessageBody
```

## Methods

### **GetJsonElement(JsonSerializerOptions)**

```csharp
Nullable<JsonElement> GetJsonElement(JsonSerializerOptions options)
```

#### Parameters

`options` JsonSerializerOptions<br/>

#### Returns

[Nullable\<JsonElement\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
